// ======================================================
// Basic shader to enable vertex colors.
// -------------------------------------
// Author: Mads Bang Hoffensetz
// StonePlant Studios
//
// This shader is here to make the vertex colored models
// in this package work. It can be used on other models,
// and the models can be used with other shaders – as
// long as vertex colors are supported.
// 
// The following notice only concerns this file.
//
// Copyright (c) 2016, StonePlant Studios
//
// Permission to use, copy, modify, and / or distribute
// this software for any purpose with or without fee is
// hereby granted, provided that the above copyright
// notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR
// DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE
// INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE
// FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL
// DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
// OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF
// CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
// ARISING OUT OF OR IN CONNECTION WITH THE USE OR
// PERFORMANCE OF THIS SOFTWARE.
// ======================================================

Shader "SPS/VertexColored"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Decal", 2D) = "black" { }
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Use a surface shader with the Lambert lighting model and let Unity
		// generate the shader code that does the heavy lifting.
		// You can replace "Lambert" with "BlinnPhong", "Standard" or "StandardSpecular".
		// Just remember to use SurfaceOutput, SurfaceOutputStandard and
		// SurfaceOutputStandardSpecular respectively in the surf function.
		#pragma surface surf Lambert vertex:vert fullforwardshadows
		#pragma target 2.0

		// The vertex shader passes vertex color to the surface shader via this struct
		struct Input
		{
			float3 vertexColor;
			float2 texcoord : TEXCOORD0;
		};

		// Tint color
		fixed4 _Color;
		sampler2D _MainTex;

		// vert function receives the vertices from Unity and passes along their color
		void vert(inout appdata_full v, out Input o)
		{
			o.vertexColor = v.color;
			o.texcoord = v.texcoord;
		}

		// surf function receives interpolated vertex color and applies it to albedo
		void surf(Input IN, inout SurfaceOutput o)
		{
			// Set albedo to the interpolated vertex color tinted by the _Color uniform
			o.Albedo = _Color.rgb * IN.vertexColor;

			float4 decalSample = tex2D(_MainTex, IN.texcoord);

			// Multiply with alpha to cancel out area's that are transparent
			float3 decalColor = lerp(o.Albedo.rgb, decalSample.rgb, decalSample.a);

			// Add to final output RGB
			o.Albedo.rgb = decalColor;

			// Set alpha to 1
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
