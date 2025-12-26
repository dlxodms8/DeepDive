Shader "UI/CircleRevealShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (0,0,0,1)
        _Cutoff ("Cutoff Value", Range(0, 1.5)) = 0
        _Smoothness ("Smoothness", Range(0, 0.5)) = 0.1
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            float _Smoothness;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                // ★ 이 줄이 추가되었습니다! (초기화 코드) ★
                UNITY_INITIALIZE_OUTPUT(v2f, OUT); 

                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 center = float2(0.5, 0.5);
                float dist = distance(center, uv);

                float alpha = smoothstep(_Cutoff, _Cutoff + _Smoothness, dist);

                fixed4 color = fixed4(IN.color.rgb, alpha);
                
                return color;
            }
            ENDCG
        }
    }
}