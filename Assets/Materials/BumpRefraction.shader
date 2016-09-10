Shader "FX/BumpRefraction" 
{
	Properties
	{
		_BumpAmt("Distortion", range(0,128)) = 10
		_BumpMap("Normalmap", 2D) = "bump" {}
		_ColorTint("Tint", Color) = (0.0, 0.0, 1.0, 0.5)
		_Shininess("Shininess", range(0,2)) = 1
		_SpecularColor("SpecularColor", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	Category
	{
		Tags
		{	"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"ForceNoShadowCasting" = "True" 
			"RenderType" = "Transparent" 
		}

		SubShader
		{
			Fog{ Mode Off }

			GrabPass
			{
				Name "BASE"
				Tags{ "LightMode" = "Always" }
			}

			Pass
			{
				Name "BASE"
				Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
	            #include "UnityLightingCommon.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
				};

				float _BumpAmt;
				float4 _BumpMap_ST;

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
					#else
					float scale = 1.0;
					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);

					return o;
				}

				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				sampler2D _BumpMap;
				fixed4 _ColorTint;
				float _Shininess;
				fixed4 _SpecularColor;

				half4 frag(v2f i) : COLOR 
				{
					half3 tNormal = UnpackNormal(tex2D(_BumpMap, i.uvbump)); 

					half2 bump = tNormal.rg; 
					float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

					half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					col *=  _ColorTint;
					 
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float specValue = dot(tNormal, lightDirection);
					col += _LightColor0 * _SpecularColor * _Shininess * specValue;
					return col;
				}
				ENDCG
			}
		}

		// ------------------------------------------------------------------
		// Fallback for older cards and Unity non-Pro

		SubShader
		{
			Blend DstColor Zero
			Pass
			{
				Name "BASE"
				SetTexture[_MainTex]{ combine texture }
			}
		}
	}
}