//Peacenet Video Decoder Shader
//
//Written by Alkaline Thunder for use in "The Peacenet". Licensed under the MIT License.
//
//See the LICENSE file in the root of https://gitlab.com/watercolor-games/the-peacenet for details.

//This shader can be used to process a compressed PNV frame and display it as a MonoGame sprite.
//
//Usage:
//1. Load the shader into an Effect object through MonoGame Content Pipeline
//2. Set the "FrameWidth" and "FrameHeight" values to the width and height values given to you by PNV.
//3. 

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

int FrameWidth = 0; //The width of each PNV frame in pixels
int FrameHeight = 0; //The height of each PNV frame in pixels

float4 PreviousFrame[65536]; //The decompressed pixel data of the previous frame.
float4 CurrentFrame[65536]; //The current, compressed PNV frame.
int PixelCounts[65536];

int CurrentPixel = 0; //The current pixel in the frame. DO NOT CHANGE THIS FROM WITHIN C#.
int PixelCount = 0; //The amount of pixels drawn on screen since the last PNV pixel - DO NOT CHANGE THIS FROM WITHIN C#.
int TotalPixelCount = 0; //Don't touch this either.

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR
{
	//grab the current PNV pixel
	float4 pixel = CurrentFrame[CurrentPixel];
	//get the previous frame's pixel at the current position
	float4 previous = PreviousFrame[TotalPixelCount];
	//create a new color
	float4 color = float4(previous.r ^ pixel.r, previous.g ^ pixel.g, previous.b ^ pixel.b, 1);
	//That's the color for our pixel.
	//Set the previous frame's pixel at this value to our pixel
	PreviousFrame[TotalPixelCount] = color;
	//Increment the Total Pixel Count.
	TotalPixelCount++;
	//Make sure it's not over the length of the array.
	if (TotalPixelCount >= FrameWidth*FrameHeight)
		TotalPixelCount = 0;
	//Now we need to increment the Current Pixel counter
	PixelCount++;
	if (PixelCount >= PixelCounts[CurrentPixel]) {
		PixelCount = 0;
		CurrentPixel++;
		if (CurrentPixel >= CurrentFrame.length)
			CurrentPixel = 0;
	}

	return color; 
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};