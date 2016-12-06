Shader "FX/Bump" 
{
	// List of all properties used by this shader
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpAmt("BumpAmount", range(-100,100)) = 1
		_Shininess("Shininess", range(0.0,1.0)) = 0.5
	}

	// groups functionality for all Subshaders
	Category
	{
		// marks shader to be processed in specific order and enables/ disables functionality
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"ForceNoShadowCasting" = "True"
			"RenderType" = "Transparent"
		}

		// contains shader logic
		SubShader
		{
			Fog{ Mode Off }

			// calculates lightrays from top to bottom
			Pass
			{
				Name "Bump"
				Tags{ "LightMode" = "ForwardBase" }
				Blend SrcAlpha OneMinusSrcAlpha			// enables alpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
	            #include "UnityLightingCommon.cginc"

				// contains mesh data
				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
					float3 normal : NORMAL;
				};

				// contains data to be passed from vertices to fragments
				struct v2f
				{
					float4 vertex : POSITION;
					float2 uvbump : TEXCOORD1;
				};

				float _BumpAmt;
				float4 _BumpMap_ST;

				// calculate fragment data from vertex data
				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
					return o;
				}

				sampler2D _BumpMap;
				float _Shininess;
				half4 _Color;

				// calculate fragment (/pixel) data from vertex data
				half4 frag(v2f i) : COLOR 
				{
					half3 bumpNormal = UnpackNormal(tex2D(_BumpMap, i.uvbump));		// get normal from BumpMap
					half2 bump = bumpNormal.rg * _BumpAmt;
					
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float specValue = dot(bump, lightDirection);
					return _Color + _LightColor0 * specValue * _Shininess;			// return fragment color
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