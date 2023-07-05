Shader "Custom/AlertOutlineShader"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _WaveAmplitude("Wave Amplitude", Range(0.1, 2)) = 1
    }
        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _WaveAmplitude;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    float waveOffset = sin(_Time.y * 10.0 + i.uv.y) * _WaveAmplitude;
                    col.rgb = float3(0.05 + waveOffset, 1.5 + waveOffset, 0.05 + waveOffset);
                    col.a *= .85;
                    return col;
                }
                ENDCG
            }
        }
}
