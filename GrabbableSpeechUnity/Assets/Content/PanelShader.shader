Shader "Unlit/PanelShader"
{
    Properties
    {
		_BackgroundColor("Background Color", Color) = (1,1,1,1)
		_ShineColor("Shine Color", Color) = (1,1,1,1)
		_ShineRamp("Shine Ramp", Float) = 1
		_FadeDown("Fade Down", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

		Pass
		{
			ZWrite Off
			Cull Front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
			};

			float4 _BackgroundColor;
			float _FadeDown;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = v.normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float fader = lerp(1, i.uv.y, _FadeDown);
				return _BackgroundColor * fader;
			}
			ENDCG
		}
        Pass
        {
			ZWrite Off
			Blend OneMinusDstColor One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				float3 objPos : TEXCOORD5;
            };

			float4 _ShineColor;
			float _ShineRamp;
			float _FadeDown;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.normal = v.normal;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.objPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float rawDot = dot(normalize(i.viewDir), normalize(i.worldNormal));
				float shine = pow(saturate(abs(rawDot)), _ShineRamp);

				float lambertShade = dot(i.worldNormal, float3(0, 1, .5)) / 2 + .5;
				float4 lambertLight = lerp(float4(.1, .1, .1, 1), float4(.3, .3, .2, 1), lambertShade);
				//return lambertLight;

				float4 shadedColor = _ShineColor * shine;

				float fader = lerp(1, i.uv.y, _FadeDown);
				return -(shadedColor + lambertLight) * fader;
            }
            ENDCG
        }
    }
}
