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
                    float3x3 TBN : TEXCOORD2;  // This takes 3 TEXCOORDs (TEXCOORD2, 3, 4)
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

                float3 ApplyCelShading(float NdotL)
                {
                    return NdotL > 0.5 ? 1.0 : 0.5;
                }

                float FogFactor(float3 worldPos)
                {
                    float distance = length(worldPos - _WorldSpaceCameraPos);
                    return saturate((distance - _FogStart) / (_FogEnd - _FogStart));
                }

                half4 frag(v2f i) : SV_Target
                {
                    // Sample normal map and convert to world space
                    float3 normalTS = UnpackNormal(tex2D(_BumpMap, i.uv));
                    float3 normalWS = normalize(mul(normalTS, i.TBN));

                    // Get lighting direction and intensity
                    Light mainLight = GetMainLight(i.shadowCoord);
                    float NdotL = saturate(dot(normalWS, mainLight.direction));

                    float cel = ApplyCelShading(NdotL);

                    float3 texColor = tex2D(_MainTex, i.uv).rgb;
                    float3 litColor = texColor * _Color.rgb * cel * mainLight.color.rgb * mainLight.shadowAttenuation;

                    // Fog
                    float fogFactor = FogFactor(i.worldPos);
                    litColor = lerp(litColor, _FogColor.rgb, fogFactor);

                    return float4(litColor, 1);
                }
                ENDHLSL
            }
        }
}