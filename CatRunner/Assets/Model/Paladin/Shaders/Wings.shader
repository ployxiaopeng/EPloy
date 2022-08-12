Shader "Custom/Wings"
{

	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MSpeed ("Main Speed", Float) = 1.0
		_SecondTex ("Second Texture", 2D) = "white" {}
		_SSpeed ("Second Speed", Float) = 1.0
		[NoScaleOffset]_MaskTex("Mask Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend One One 
		Cull Off
		ZWrite Off
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

			sampler2D	_MainTex, _SecondTex, _MaskTex;;
			float4	_MainTex_ST, _SecondTex_ST, _Color; 
			fixed	_MSpeed, _SSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 mainUV = TRANSFORM_TEX(i.uv, _MainTex);
				fixed4 col = tex2D(_MainTex, mainUV - float2(1, 0) * _MSpeed * _Time.x);
				fixed4 scol = tex2D(_SecondTex, mainUV - float2(1, 0) * _SSpeed * _Time.x);
				fixed4 maskCol = tex2D(_MaskTex, i.uv);
				return (col + scol) * maskCol * _Color;
			}
			ENDCG
		}
	}
}
