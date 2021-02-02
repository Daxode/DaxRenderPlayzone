Shader "Unlit/Grass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GausX ("GausX", Vector) = (0.74,0.5,0.01,0)
        _GausY ("GausY", Vector) = (0.65,0.01,0.3,0.06)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GausX;
            float4 _GausY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                // sample the texture
                //float brightness = (sin((i.uv.x-0.25)*UNITY_TWO_PI)+1)*0.5;
                //float brightness = sin(i.uv.x*UNITY_PI);
                float3 baseWorldPos = unity_ObjectToWorld._m30_m31_m32;
                float shift = (sin(i.uv.y*UNITY_TWO_PI+_Time.w+baseWorldPos.z*10)+1)*0.1;
                
                float valOne = ((i.uv.y)-_GausY.y);
                float brightness = _GausY.x*exp(-(valOne*valOne)/_GausY.z);
                
                valOne = ((i.uv.x-shift)-_GausX.y);
                brightness = brightness*_GausX.x*exp(-(valOne*valOne)/_GausX.z);
                
                clip(brightness-_GausY.w);
                brightness = clamp(brightness,0, 1);
                brightness *= 4;

                fixed4 col = lerp(float4(0.5,0.7,0.1,1), float4(0.2,0.8,0,1),brightness);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
