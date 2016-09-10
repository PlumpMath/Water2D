Shader "FX/VolumetricDirectionalLight"
{
	Properties
	{
		_Color ("LightColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_Speed("Speed", range(0.0,1.0)) = 0.5
		_Falloff("Falloff", range(0.0,1.0)) = 0.5
		_LightMap ("Light Normal Map", 2D) = "bump" {}
	}
	Category
	{
		Tags
		{ "Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"ForceNoShadowCasting" = "True"
			"RenderType" = "Transparent"
		}

		SubShader
		{
			Pass
			{
				Name "BASE"
				Tags{ "LightMode" = "ForwardBase" }

				Fog { Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 uvLight : TEXCOORD1;
				};

				float4 _LightMap_ST;
				float _Speed;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.texcoord;
					o.uvLight.x = TRANSFORM_TEX(v.texcoord, _LightMap).x;
					o.uvLight.y = ((_SinTime.w * _Speed) * 0.5) + 0.5;
					return o;
				}

				sampler2D _LightMap;
				float _Falloff;
				fixed4 _Color;

				half4 frag(v2f i) : COLOR
				{
					half3 tNormal = UnpackNormal( tex2D(_LightMap, i.uvLight) );
				
					half4 col = tex2D(_LightMap, i.uvLight);
					col *= _LightColor0 * _Color;
					col = lerp( half4(0.0, 0.0, 0.0, 0.0), col, i.uv.y * _Falloff);
					return col;
				}
				ENDCG
			}
		}
	}
}
