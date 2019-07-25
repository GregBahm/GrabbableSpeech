Shader "Unlit/SummonerHighlightCircle"
{
	Properties
	{
		_HighlightProg("HighlightProg", Range(0, 1)) = .5
		_ShadowColor("Shadow Color", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
		_Softness("Softness", Float) = 1
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 100

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
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
					float3 viewDir : TEXCOORD2;
					float3 worldNormal : TEXCOORD3;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				float3 _ShadowColor;
				float4 _Color;
				float _Softness;
				float _HighlightProg;
				float _Hover;

				v2f vert(appdata v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.normal = v.normal;
					o.viewDir = WorldSpaceViewDir(v.vertex);

					o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
					return o;
				}

				float GetShadowAlpha(float2 uv)
				{
					float centerDist = length((uv - .5)) * 2;
					float alpha = saturate((.7 - centerDist) * 5);
					alpha *= uv.x * 2 - .5;
					alpha = saturate(alpha);
					return alpha;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(i);
					float centerDist = length((i.uv - .5)) * 2;

					float highlightness = max(_Hover / 4, _HighlightProg);
					//return _HighlightProg;
					float circleRadius = lerp(.5, 1, highlightness);
					float circleProgAlpha = (1 - pow(highlightness, 2));
					float alpha = saturate((circleRadius - centerDist) * _Softness);
					float4 ret = float4(_Color.xyz, alpha * circleProgAlpha);

					float shadowAlpha = GetShadowAlpha(i.uv);
					float4 shadow = float4(_ShadowColor, shadowAlpha);
					ret = lerp(ret, shadow, shadowAlpha);
					return ret;
				}
				ENDCG
			}
		}
}
