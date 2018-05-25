Shader "Custom/Stencil/Diffuse EqualOne fix"
{

Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}

	_EmissionLM("Emission (Lightmapper)", Float) = 0
	[Toggle] _DynamicEmissionLM("Dynamic Emission (Lightmapper)", Int) = 0
}

SubShader
{
	Tags { "RenderType"="Opaque" "Queue"="Geometry" }
	LOD 200

	// Only render pixels whose value in the stencil buffer equals 1.
	Stencil
	{
		Ref 1
		Comp equal
		Pass keep
	}

	CGPROGRAM
	#pragma surface surf Lambert

	sampler2D _MainTex;
	fixed4 _Color;

	struct Input
	{
		float2 uv_MainTex;
	};
	struct Output {
		o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	};
	void surf (Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	

	ENDCG
}

Fallback "VertexLit"
}
