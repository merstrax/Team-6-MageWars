Shader "Custom/OutlineShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range (0, 0.1)) = 0.01
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            // Outline pass
            Name "OUTLINE"
            Cull Front  // Render the backfaces to simulate an outline
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            v2f vert(appdata v)
            {
                // Inflate vertex along normal direction to create outline
                v2f o;
                float3 norm = normalize(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex + norm * _OutlineWidth);
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }

        // Regular rendering pass for the object
        
    }
    
    FallBack "Universal Render Pipeline/Lit"
}