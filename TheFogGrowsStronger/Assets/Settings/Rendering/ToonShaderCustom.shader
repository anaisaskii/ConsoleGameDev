Shader "Custom/ToonShaderCustom"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {} //base texture
        _BumpMap("Normal Map", 2D) = "bump" {} //normal map
        _Color("Color", Color) = (1, 1, 1, 1) //object colour
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1) //fog colour
        _FogStart("Fog Start", Float) = 10 //where fog starts
        _FogEnd("Fog End", Float) = 50 //where fog ends
        _Brightness("Brightness", Range(0, 2)) = 1 //colour/texture brightness
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1) //shadow colour
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5 //how much shadow to display
        _ShadowBlend("Shadow Blend", Range(0.001, 1)) = 0.1 // how much to blend the shadow
        [Toggle(_USE_DIRECTIONAL_LIGHTING)] _UseDirectionalLighting("Use Directional Lighting", Float) = 1 //directional lighting toggle
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
                #pragma multi_compile_instancing
                #pragma instancing_options renderinglayer
                #pragma shader_feature_local _USE_DIRECTIONAL_LIGHTING

                //urp stuff
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

                //instancing buffer
                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_INSTANCING_BUFFER_END(Props)

                //vertex input
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                //vertex to fragment
                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                    float3x3 TBN : TEXCOORD2;
                    float4 shadowCoord : TEXCOORD5;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                //variables
                sampler2D _MainTex;
                sampler2D _BumpMap;
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _FogColor;
                float _FogStart;
                float _FogEnd;
                float _Brightness;
                float4 _ShadowColor;
                float _ShadowThreshold;
                float _ShadowBlend;

                //vertex shader
                //converts meshes to world space
                //sets UVs
                v2f vert(appdata v)
                {
                    UNITY_SETUP_INSTANCE_ID(v);
                    v2f o;
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldPos = TransformObjectToWorld(v.vertex.xyz);

                    float3 worldNormal = TransformObjectToWorldNormal(v.normal);
                    float3 worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
                    float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                    o.TBN = float3x3(worldTangent, worldBinormal, worldNormal);
                    o.shadowCoord = TransformWorldToShadowCoord(o.worldPos);

                    return o;
                }

                // Sets fog attributes
                float FogFactor(float3 worldPos)
                {
                    float distance = length(worldPos - _WorldSpaceCameraPos);
                    return saturate((distance - _FogStart) / (_FogEnd - _FogStart));
                }

                //fragment shader
                half4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(i);

                    float3 normalTS = UnpackNormal(tex2D(_BumpMap, i.uv));
                    float3 normalWS = normalize(mul(normalTS, i.TBN));

                    float NdotL;
                    float3 lightColor = 1.0;
                    float shadowAttenuation = 1.0;

                    //check if using directional lighting or not
                    //get directional light attributes
                    #ifdef _USE_DIRECTIONAL_LIGHTING
                        Light mainLight = GetMainLight(i.shadowCoord);
                        NdotL = saturate(dot(normalWS, mainLight.direction));
                        lightColor = mainLight.color.rgb; // add light colour to texture
                        shadowAttenuation = mainLight.shadowAttenuation;
                    #else
                        //'fake light' - generic top down light
                        float3 fakeLightDir = normalize(float3(0.0, -1.0, 0.0)); // light from above
                        NdotL = saturate(dot(normalWS, -fakeLightDir)); // light hitting top surfaces
                    #endif

                    // toon shadows
                    float lightAmount = step(_ShadowThreshold, NdotL);
                    // interpolate between colour and light
                    float3 shadowTint = lerp(_ShadowColor.rgb, 1.0, lightAmount);
                    // smooth shadows
                    float blendedShadow = smoothstep(_ShadowThreshold - _ShadowBlend, _ShadowThreshold + _ShadowBlend, NdotL);
                    shadowTint = lerp(shadowTint, 1.0, blendedShadow);

                    // get instance and texture color
                    float4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                    float3 texColor = tex2D(_MainTex, i.uv).rgb;

                    // calculates lit colour using shadow tint and light properties
                    float3 litColor = shadowTint * lightColor * shadowAttenuation;
                    // combine texture, colour, lighting, and brightness
                    float3 finalColor = texColor * instanceColor.rgb * litColor * _Brightness;

                    // blends fog into colour for final output
                    float fogFactor = FogFactor(i.worldPos); 
                    finalColor = lerp(finalColor, _FogColor.rgb, fogFactor);

                    return float4(finalColor, 1);
                }
                ENDHLSL
            }
        }
}
