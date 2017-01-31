Shader "Hidden/CustomBloom"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

		CGINCLUDE

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _ColorBuffer;
	sampler2D _MainTex;

	half _Intensity;
	half4 _ColorBuffer_ST;
	half4 _MainTex_ST;

	half4 fragAddCheap(v2f i) : SV_Target
	{
		half4 addedbloom = tex2D(_MainTex, i.uv);
		half4 screencolor = tex2D(_ColorBuffer, i.uv);
		return _Intensity * addedbloom + screencolor;
	}

		ENDCG

		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment fragAddCheap

			ENDCG
		}
	}
	Fallback off
}
