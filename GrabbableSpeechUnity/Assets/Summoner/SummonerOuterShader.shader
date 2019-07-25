Shader "Unlit/SummonerOuterShader"
{
    Properties
    {
		_BigArrowTex("Big Arrow Tex", 2D) = "black" {}
		_ArrowsAlpha("Arrows Alpha", Range(0, 1)) = 1
		_ArrowsColor("Arrows", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 objSpace : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			sampler2D _BigArrowTex;
			float _ArrowsAlpha;
			float4 _ArrowsColor;
			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = v.normal;
				o.objSpace = v.vertex;
                return o;
            }

			float GetArrowsTex(float3 objSpace)
			{
				float2 xy = abs(objSpace.xz) * 100;
				float maskA = min(xy.x, xy.y) * 10;
				maskA = (maskA - 6.5) * 20;
				float2 inner = .3 - abs(xy * .6 - .5) * 10;
				inner = saturate(inner * 10);
				float maskB = max(inner.x, inner.y);

				float maskC = max(xy.x, xy.y);
				maskC = maskC * 20 - 15.5;
				maskC = 1 - saturate(maskC);
				return saturate(maskC * maskA * maskB);
			}

			float GetBigArrow(float3 objSpace)
			{
				float2 uvs = (objSpace.xz + float2(.005, -.011)) * 100;
				fixed ret = tex2D(_BigArrowTex, uvs).x;
				return ret;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);

				float bigArrowTex = GetBigArrow(i.objSpace);
				float arrowsTex = GetArrowsTex(i.objSpace);
				float shade = i.normal.x / 2 + .5;
				shade = 1.5 - shade;
				shade *= saturate(pow(saturate(i.normal.y), 2) + .5);

				float4 bigArrow = lerp(_Color, _ArrowsColor, bigArrowTex);
				float4 littleArrows = lerp(_Color, _ArrowsColor, arrowsTex);
				float4 ret = lerp(bigArrow, littleArrows, _ArrowsAlpha);
				ret *= shade;
				return ret;
            }
            ENDCG
        }
    }
}
