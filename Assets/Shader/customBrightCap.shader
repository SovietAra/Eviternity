Shader "Hidden/customBrightCap"
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
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.uv;
		return o;
	}

	half _Threshold;
	sampler2D _MainTex;

	half4 frag(v2f i) : SV_Target
	{
		half4 col = tex2D(_MainTex, i.uv);
		//if (col.r < _Threshold && col.g < _Threshold && col.b < _Threshold)
			//col = half4(0, 0, 0, 0);
		half substract = 1 - _Threshold;
		half4 orig_col = col;
		col = max(col - half4(substract, substract, substract, substract), half4(0, 0, 0, 0));
		half4 multiplier = half4(orig_col.r / (orig_col.r - substract), orig_col.g / (orig_col.g - substract), orig_col.b / (orig_col.b - substract), orig_col.a / (orig_col.a - substract));
		col = half4(col.r * multiplier.r, col.g * multiplier.g, col.b * multiplier.b, col.a * multiplier.a);
		return col;
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
			#pragma fragment frag

			ENDCG
		}
	}
	Fallback off
}
