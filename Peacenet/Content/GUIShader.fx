//Peace Engine GUI Shader
//
//This shader allows certain effects such as the gray-out of disabled UI elements, semi-translucency, etc to be done on the GPU rather than on the CPU.
//
//This allows us to not use RenderTargets as often as we do, thus saving precious GPU resources and not killing my onboard AMD GPU's rendertarget limit.
//
//Written by The Fuzzy Riolu.

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Sampler that allows us to grab pixel data of the screen.
sampler TextureSampler : register(s0);

//Is the UI element that's rendering enabled?
bool Enabled = true;

//The color used for tinting.
float4 TintColor;

float4 MainPS(float4 texCoord : TEXCOORD0) : COLOR
{
	//grab the color of the texture at the current coordinates
	float4 color = tex2D(TextureSampler, texCoord);

	//Multiply the color by the data in our VS input
	color.r = mul(color.r, TintColor.r);
	color.g = mul(color.r, TintColor.g);
	color.b = mul(color.r, TintColor.b);
	color.a = mul(color.r, TintColor.a);

	return color;
}

float4 GrayOut(float2 texCoord : TEXCOORD0) : COLOR
{
	//get the current color
	float4 color = tex2D(TextureSampler, texCoord);
	//if we're not enabled, gray-out.
	if (Enabled == false)
	{
		color.r = color.r * 0.5;
		color.g = color.g * 0.5;
		color.b = color.b * 0.5;
	}
	return color;
}

technique GUIShader
{
	//First pass applies the tinting of textures.
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass P1
	{
		PixelShader = compile PS_SHADERMODEL GrayOut();
	}
};