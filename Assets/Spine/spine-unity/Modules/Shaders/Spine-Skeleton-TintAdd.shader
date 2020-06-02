// Spine/Skeleton Tint
// - Two color tint
// - unlit
// - Premultiplied alpha blending
// - No depth, no backface culling, no fog.

Shader "Spine/Skeleton TintAdd" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_Black ("Black Point", Color) = (0,0,0,0)
		_alpha ("Alpha",Range(0,1)) = 1
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "black" {}
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _Color;
			float4 _Black;
			float _alpha;

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
				float4 col : COLOR1;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex); // replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
				o.uv = v.uv;
				//o.vertexColor = v.vertexColor * float4(_Color.rgb * _Color.a, _Color.a); // Combine a PMA version of _Color with vertexColor.
				o.vertexColor = v.vertexColor;
				o.col = o.vertexColor.a * _Color;
				//o.vertexColor1 = float4(_Color.rgb * _Color.a, _Color.a) * v.vectexColor.a;
				//o.vertexColor = _Color;
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				float4 texColor = tex2D(_MainTex, i.uv);
				//return texColor  +  texColor *( i.vertexColor * texColor.a) ;
				//return texColor * i.vertexColor + texColor * i.vertexColor.a ;
				//return texColor *( i.vertexColor * texColor.a)  * (_Color * texColor.a ) ;
				_Black.a = 1;
				if(_Black.r <0.01 && _Black.g < 0.01 && _Black.b < 0.01)
					_Black = float4(1,1,1,1);
				return (texColor * (i.vertexColor * texColor.a)* _Black + i.col * texColor.a)* _alpha;
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			ZWrite On
			ZTest LEqual

			Fog { Mode Off }
			Cull Off
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed _Cutoff;

			struct VertexOutput { 
				V2F_SHADOW_CASTER;
				float2 uv : TEXCOORD1;
			};

			VertexOutput vert (appdata_base v) {
				VertexOutput o;
				o.uv = v.texcoord;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag (VertexOutput i) : COLOR {
				fixed4 texcol = tex2D(_MainTex, i.uv);
				clip(texcol.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
