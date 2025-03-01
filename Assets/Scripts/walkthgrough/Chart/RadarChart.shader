Shader "UI/RadarChart"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillColor ("Fill Color", Color) = (1,0,0,0.5)
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.005
        _Value1 ("Value 1", Range(0, 1)) = 0.8
        _Value2 ("Value 2", Range(0, 1)) = 0.7
        _Value3 ("Value 3", Range(0, 1)) = 0.6
        _Value4 ("Value 4", Range(0, 1)) = 0.5
        _Value5 ("Value 5", Range(0, 1)) = 0.9
        
        // UI compatibility properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _FillColor;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _Value1;
            float _Value2;
            float _Value3;
            float _Value4;
            float _Value5;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // Convert to -1 to 1 range
                o.color = v.color;
                return o;
            }
            
            float2 rotatePoint(float2 p, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                return float2(p.x * c - p.y * s, p.x * s + p.y * c);
            }
            
            // Distance from point to line segment
            float distToLine(float2 p, float2 a, float2 b)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                float2 closestPoint = a + t * ba;
                return length(p - closestPoint);
            }
            
            float isInPolygon(float2 testPoint, float2 vertices[5])
            {
                int i, j;
                bool inside = false;
                for (i = 0, j = 4; i < 5; j = i++) {
                    if (((vertices[i].y > testPoint.y) != (vertices[j].y > testPoint.y)) &&
                        (testPoint.x < (vertices[j].x - vertices[i].x) * (testPoint.y - vertices[i].y) / 
                        (vertices[j].y - vertices[i].y) + vertices[i].x))
                    {
                        inside = !inside;
                    }
                }
                return inside ? 1.0 : 0.0;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 points[5];
                
                // Calculate pentagon points based on values - start from top and go clockwise
                points[0] = float2(0, _Value1);  // Top
                points[1] = rotatePoint(float2(0, _Value2), radians(72));   // Top-Right
                points[2] = rotatePoint(float2(0, _Value3), radians(144));  // Bottom-Right
                points[3] = rotatePoint(float2(0, _Value4), radians(216));  // Bottom-Left
                points[4] = rotatePoint(float2(0, _Value5), radians(288));  // Top-Left
                
                // First check if inside the polygon
                float inFill = isInPolygon(i.uv, points);
                
                // Then check if on any outline
                float onOutline = 0;
                for (int j = 0; j < 5; j++) {
                    int next = (j + 1) % 5;
                    float dist = distToLine(i.uv, points[j], points[next]);
                    if (dist < _OutlineWidth) {
                        onOutline = 1;
                        break;
                    }
                }
                
                // Combine fill and outline
                float4 color = inFill * _FillColor * (1 - onOutline) + onOutline * _OutlineColor;
                
                // Only return color if inside or on outline
                return color * (max(inFill, onOutline));
            }
            ENDCG
        }
    }
}