Shader "FX/BumpRefractionLightRay"
{
	// List of all properties used by this shader
	Properties
	{
		_BumpMap0("Bump Map Base ", 2D) = "bump" {}
		_BumpMap1("Bump Map Target ", 2D) = "bump" {}
		_BumpAmt("Bump Amount", range(0,128)) = 10
		_BumpSpeed("Bump Speed", range(0.0,1.0)) = 0.5
		
		_TintTop("Tint Top", Color) = (0.0, 0.0, 1.0, 0.5)
		_TintBot("Tint Bottom", Color) = (0.0, 0.0, 0.5, 0.8)
		
		_SpecIntens("Specular Intensity", range(0.0,2.0)) = 0.5
		_SpecuHeight("Specular Height", range(0.0,1.0)) = 1.0
		_SpecuFalloff("Specular Falloff", range(0.0,1.0)) = 1.0
		_SpecuColor("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)

		_RaySpeed ("Ray Speed", range(0.0,1.0)) = 0.5
		_RayHeight("Ray Height", range(0.0,1.0)) = 0.5
		_RayFalloff ("Ray Falloff", range(0.0,1.0)) = 0.5
		_RayColor("Ray Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}

	// groups functionality for all Subshaders
	Category
	{
		// marks shader to be processed in specific order and enables/ disables functionality
		Tags
		{	"Queue" = "Transparent"
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
				Name "LightRay"
				Tags{ "LightMode" = "Always" }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				#include "ShaderExtension.cginc"	
				
				// contains mesh data
				struct appdata
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
				};
			
				// contains data t be passed from vertices to fragments
				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 uvRay : TEXCOORD1;
				};

				float4 _BumpMap1_ST;
				float _RaySpeed;

				// vertex function to be called on all vertices
				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);					// model view projection
					o.uv = v.texcoord;											
					o.uvRay.x = TRANSFORM_TEX(v.texcoord, _BumpMap1).x;			// set texture coordinate from bump map
					o.uvRay.y = ((_SinTime.w * _RaySpeed) * 0.5) + 0.5;			// set texture coordinate from SinTime
					return o;
				}

				sampler2D _BumpMap1;
				float _RayHeight;
				float _RayFalloff;
				fixed4 _RayColor;

				// calculate fragment data from vertex data
				half4 frag(v2f i) : COLOR
				{
					// discard fragment, if too close to bottom
					if (i.uv.y < _RayHeight) 
					{
						discard;
					}

					half4 col = tex2D(_BumpMap1, i.uvRay);						// sets color and alpha parameter
					col *= _LightColor0;
					col += _RayColor;
					col = GetFalloff( col, i.uv.y, _RayHeight, _RayFalloff );
					return col;
				}
				ENDCG
			}

			// ------------------------------------------------------------------
			// saves currently rendered texture 
			GrabPass
			{
				Name "BASE"
				Tags{ "LightMode" = "Always" }
			}
			
			// main pass calculating: bumpiness (waves), reflection, refraction
			Pass
			{
				Name "BASE"
				Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				#include "ShaderExtension.cginc"

				// contains mesh data
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
				};

				// contains data to be passed from vertices to fragments
				struct v2f
				{
					float4 vertex : POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
					float2 uv : TEXCOORD2;
				};

				float _BumpAmt;
				float4 _BumpMap0_ST;
				float _BumpSpeed;

				// calculate vertex data to be passed to fragement function
				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
					#else
					float scale = 1.0;
					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;	// set grab texture UVs
					o.uvgrab.zw = o.vertex.zw;
					o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap0);
					o.uv = v.texcoord;
					return o;
				}

				sampler2D _GrabTexture;				// grapped texture from pass above
				float4 _GrabTexture_TexelSize;		// texture pixel
				sampler2D _BumpMap0;
				sampler2D _BumpMap1;
				fixed4 _TintTop;
				fixed4 _TintBot;
				float _SpecIntens;
				fixed4 _SpecuColor;
				float _SpecuHeight;
				float _SpecuFalloff;

				// calculate fragment (/pixel) data from vertex data
				half4 frag(v2f i) : COLOR
				{
					half3 bumpNormal0 = UnpackNormal(tex2D(_BumpMap0, i.uvbump));		// interpolation by t between BumpMap0 and BumpMap1
					half3 bumpNormal1 = UnpackNormal(tex2D(_BumpMap1, i.uvbump));
					float t = (((_SinTime.w * _BumpSpeed) * 0.5) + 0.5);
					half2 bump = lerp(bumpNormal0.rg, bumpNormal1.rg, t);

					// handle refraction using bump
					float2 offsetVec = bump * _BumpAmt *_GrabTexture_TexelSize.xy;		// convert to GrabTexture Space
					i.uvgrab.xy = i.uvgrab.xy + offsetVec * i.uvgrab.z;					// moves xy position of grab texture
					half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));	// sampler lookup

					// set tint
					col *= lerp(_TintBot, _TintTop, i.uv.y);

					// handles specular lighting
					if (i.uv.y > _SpecuHeight) 
					{
						float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
						float specValue = dot(bump, lightDirection);
						half4 addCol = _SpecuColor + (_LightColor0 * _SpecIntens * specValue);
						col += GetFalloff( addCol, i.uv.y, _SpecuHeight, _SpecuFalloff );
					}
					return col;
				}
				ENDCG
			}
		}

		// ------------------------------------------------------------------
		// Fallback for older cards
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