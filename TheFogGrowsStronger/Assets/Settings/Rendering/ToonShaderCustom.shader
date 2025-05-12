Shader "Custom/ToonShaderCustom"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogStart("Fog Start", Float) = 10
        _FogEnd("Fog End", Float) = 50
        _Brightness("Brightness", Range(0, 2)) = 1
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowBlend("Shadow Blend", Range(0.001, 1)) = 0.1
    }

        SubShader
        {
            Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

            Pass
            {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                    float3x3 TBN : TEXCOORD2;
                    float4 shadowCoord : TEXCOORD5;
                };

                sampler2D _MainTex;
                sampler2D _BumpMap;
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _Color;
                float4 _FogColor;
                float _FogStart;
                float _FogEnd;
                float _Brightness;
                float4 _ShadowColor;
                float _ShadowThreshold;
                float _ShadowBlend;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                    float3 worldNormal = TransformObjectToWorldNormal(v.normal);
                    float3 worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
                    float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                    o.TBN = float3x3(worldTangent, worldBinormal, worldNormal);

                    // Shadows
                    o.shadowCoord = TransformWorldToShadowCoord(o.worldPos);

                    return o;
                }

                float FogFactor(float3 worldPos)
                {
                    float distance = length(worldPos - _WorldSpaceCameraPos);
                    return saturate((distance - _FogStart) / (_FogEnd - _FogStart));
                }

                half4 frag(v2f i) : SV_Target
                {
                    float3 normalTS = UnpackNormal(tex2D(_BumpMap, i.uv));
                    float3 normalWS = normalize(mul(normalTS, i.TBN));

                    Light mainLight = GetMainLight(i.shadowCoord);
                    float NdotL = saturate(dot(normalWS, mainLight.direction));

                    // Shadow threshold logic
                    float lightAmount = step(_ShadowThreshold, NdotL); // 0 or 1 based on threshold
                    float3 shadowTint = lerp(_ShadowColor.rgb, 1.0, lightAmount);

                    // Shadow Blend logic for smoother shadow transition
                    float blendedShadow = smoothstep(_ShadowThreshold - _ShadowBlend, _ShadowThreshold + _ShadowBlend, NdotL);
                    shadowTint = lerp(shadowTint, 1.0, blendedShadow);  // Blend between shadow and light using the ShadowBlend factor

                    // Apply texture color and brightness
                    float3 texColor = tex2D(_MainTex, i.uv).rgb;
                    float3 finalColor = texColor * _Color.rgb * shadowTint * mainLight.color.rgb * mainLight.shadowAttenuation * _Brightness;

                    // Fog effect
                    float fogFactor = FogFactor(i.worldPos);
                    finalColor = lerp(finalColor, _FogColor.rgb, fogFactor);

                    return float4(finalColor, 1);
                }
                ENDHLSL
            }
        }
}
