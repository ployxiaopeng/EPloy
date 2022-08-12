Shader "Custom/Particle_Dissolve"
{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" } 
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha 
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
				float3 uv : TEXCOORD0;	  
				fixed4 color : COLOR;	  
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;        
				float4 vertex : SV_POSITION;  
				fixed4 color : COLOR;	

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;	

			v2f vert (appdata v)
			{	
				v2f o;
				o.color = v.color;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
				o.uv.z = v.uv.z;		
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float CustomData1 = i.uv.z;	
				fixed4 col = tex2D(_MainTex, i.uv);
				float4 finalColor;
				finalColor.rgb = i.color.rgb;
				finalColor.a = ceil(col.r - CustomData1) * i.color.a;	
				return finalColor;
			}
			ENDCG
		}
	}
}
