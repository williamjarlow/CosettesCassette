
Shader "Custom/LockedTexReplaceSprite" {
	Properties{
		[PerRendererData] _MainTex("Tex For Alpha (RGB)", 2D) = "white" {}
		_TextureSlot1("TextureSlot1 (RGB)", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}
	SubShader{
		Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"CanUseSpriteAtlas" = "True"
			"PreviewType" = "Plane"
		}
		Cull Off
		Lighting Off
		Zwrite Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		//Surface shader

		#pragma surface surf Lambert alpha
		sampler2D _MainTex;
		sampler2D _TextureSlot1;

		struct Input {
			float2 uv_MainTex;
			float2 uv_TextureSlot1;
			float2 uv_TextureSlot2;
			float2 uv_TextureSlot3;
			float4 screenPos;
		};

		void surf(Input IN, inout SurfaceOutput output) {
			half2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			half4 primary = tex2D(_MainTex, IN.uv_MainTex);
			half4 slot1 = tex2D(_TextureSlot1, screenUV);

			output.Albedo = slot1.rgb;

			output.Alpha = primary.a; //Keep sprite transparency
			output.Albedo = output.Albedo * 5; //Restore normal brightness, don't know why
		}
		ENDCG
	}
	//This fallback shader completely discards the effect, and isn't really acceptable, but oh well
	FallBack "Diffuse"
}