Shader "Custom/DayLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DayLight ("Day Light", Color) = (1, 1, 1, 1)
		_BuffColor ("Buff Color", Color) = (0.5, 0.5, 0.5, 1)
		_CutOff ("Cut Off", Range(0, 1)) = 0.5
		_Alpha ("Alpha", Color) = (0, 0, 0, 1)
	}
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _DayLight;
		float4 _BuffColor;
		float _CutOff;
		float4 _Alpha;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			clip(c.a - _CutOff);
			
			c.a = c.a * _Alpha.a;// * _DayLight.a;
			
			o.Albedo = c.rgb * _BuffColor.rgb * _DayLight.rgb;
			o.Emission = o.Albedo * 2;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		Lighting Off
		
		Alphatest Greater [_Cutoff]
		
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
			Lighting Off
			
			SetTexture [_MainTex] {
				ConstantColor[_BuffColor] 
				Combine texture * constant
			}
			
			SetTexture [_MainTex] {
				ConstantColor[_DayLight] 
				Combine previous * constant
			}
			
			SetTexture [_MainTex] {
				ConstantColor[_Alpha] 
				Combine previous DOUBLE, texture * constant
			}
		}
	
	} 
	FallBack "Unlit/Transparent"
}
