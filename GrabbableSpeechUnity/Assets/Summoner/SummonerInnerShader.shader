Shader "Unlit/SummonerInnerShader"
{
    Properties
    {
		_Activeness("Activeness", Range(0, 1)) = 0
		_StartingColor("Starting Color", Color) = (1,1,1,1)
		_ActiveHighColor("Active High Color", Color) = (1,1,1,1)
		_ActiveLowColor("Active Low Color", Color) = (1,1,1,1)
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			float _Activeness;
			float4 _StartingColor;
			float4 _ActiveHighColor;
			float4 _ActiveLowColor;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = v.normal;
                return o;
            }

			float4 GetColor(float uvY, float3 normal)
			{
				float startingKey = _Activeness;
				startingKey *= abs(normal.y);
				//startingKey *= (1 - normal.x);
				float4 activeColor = lerp(_ActiveLowColor, _ActiveHighColor, uvY);
				return lerp(_StartingColor, activeColor, startingKey);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
				float4 color = GetColor(i.uv.y, i.normal);
				return color;
            }
            ENDCG
        }
    }
}
