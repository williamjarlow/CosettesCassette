Shader "Custom/Stencil/Mask OneZLess"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }	// Write to the stencil buffer before drawing any geometry to the screen
        ColorMask 0	// Don't write to any colour channels
        ZWrite off	// Don't write to the Depth buffer
        
		// Write the value 1 to the stencil buffer
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }
        
        Pass
        {
            Cull Back
            ZTest Less
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : COLOR
            {
                return half4(1,1,0,1);
            }
            
            ENDCG
        }
    } 
}