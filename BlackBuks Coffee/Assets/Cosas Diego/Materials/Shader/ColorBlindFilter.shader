Shader "Custom/ColorblindFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mode ("Type (0: Normal, 1: Protanopia, 2: Deuteranopia, 3: Tritanopia)", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            int _Mode;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 c = col.rgb;

                if (_Mode == 1) // Protanopia
                {
                    col.r = 0.56667 * c.r + 0.43333 * c.g;
                    col.g = 0.55833 * c.r + 0.44167 * c.g;
                    col.b = 0.24167 * c.g + 0.75833 * c.b;
                }
                else if (_Mode == 2) // Deuteranopia
                {
                    col.r = 0.62500 * c.r + 0.37500 * c.g;
                    col.g = 0.70000 * c.r + 0.30000 * c.g;
                    col.b = 0.30000 * c.g + 0.70000 * c.b;
                }
                else if (_Mode == 3) // Tritanopia
                {
                    col.r = 0.95000 * c.r + 0.05000 * c.g;
                    col.g = 0.43333 * c.g + 0.56667 * c.b;
                    col.b = 0.47500 * c.g + 0.52500 * c.b;
                }

                return col;
            }
            ENDCG
        }
    }
}