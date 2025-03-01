Shader "UI/PentagonGrid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridColor ("Grid Color", Color) = (0.8, 0.8, 0.8, 0.5)
        _LineWidth ("Line Width", Range(0.001, 0.01)) = 0.003
        _Divisions ("Divisions", Range(1, 5)) = 3
        
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
            float4 _GridColor;
            float _LineWidth;
            int _Divisions;
            
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
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Base alpha (transparent)
                float alpha = 0;
                
                // Points for pentagon vertices at full radius (1.0)
                float2 points[5];
                for (int p = 0; p < 5; p++)
                {
                    float angle = radians(p * 72 + 90); // Start from top, go clockwise
                    points[p] = float2(cos(angle), sin(angle));
                }
                
                // Draw concentric pentagons
                for (int d = 1; d <= _Divisions; d++)
                {
                    float radius = d / float(_Divisions);
                    
                    float2 scaledPoints[5];
                    for (int p = 0; p < 5; p++)
                    {
                        scaledPoints[p] = points[p] * radius;
                    }
                    
                    // Check distance to each line segment of the pentagon
                    for (int p = 0; p < 5; p++)
                    {
                        int nextPoint = (p + 1) % 5;
                        float lineDistance = distToLine(i.uv, scaledPoints[p], scaledPoints[nextPoint]);
                        
                        // If close to line, draw it
                        if (lineDistance < _LineWidth)
                        {
                            alpha = 1;
                        }
                    }
                }
                
                // Draw lines from center to vertices
                for (int p = 0; p < 5; p++)
                {
                    float lineDistance = distToLine(i.uv, float2(0,0), points[p]);
                    if (lineDistance < _LineWidth)
                    {
                        alpha = 1;
                    }
                }
                
                return float4(_GridColor.rgb, _GridColor.a * alpha);
            }
            ENDCG
        }
    }
}