Shader "Custom/ToonShaderCustom"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogStart("Fog Start", Float) = 10
        _FogEnd("Fog End", Float) = 50
    }
        SubShader
        {
            Tags { "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _Color;
                float4 _FogColor;
                float _FogStart;
                float _FogEnd;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                float3 ApplyCelShading(float3 normal, float3 lightDir)
                {
                    float NdotL = dot(normalize(normal), normalize(lightDir));
                    return NdotL > 0.5 ? 1 : 0.5; // Two-tone cel shading
                }

                float FogFactor(float3 worldPos)
                {
                    float distance = length(worldPos - _WorldSpaceCameraPos);
                    return saturate((distance - _FogStart) / (_FogEnd - _FogStart));
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float3 normal = float3(0, 0, 1); // flat for now, no normal map
                    float3 lightDir = _WorldSpaceLightPos0.xyz;

                    float celShade = ApplyCelShading(normal, lightDir);
                    float3 color = tex2D(_MainTex, i.uv).rgb * _Color.rgb * celShade;

                    // Fog
                    float fogFactor = FogFactor(i.worldPos);
                    color = lerp(color, _FogColor.rgb, fogFactor);

                    return float4(color, 1);
                }
                ENDCG
            }
        }
}
