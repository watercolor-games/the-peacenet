//WorldMapShader - written by Alkaline Thunder.
//
//This shader is used by the in-game World Map to shade map textures (in binary color) with the game's theme colors using the GPU.

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 ThemeBorderColor;
float4 ThemeLandColor;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 texColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);
	if(texColor.r == 1 && texColor.g == 1 && texColor.b == 1)
		return ThemeBorderColor * input.Color.a;
	else
		return ThemeLandColor * input.Color.a;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};