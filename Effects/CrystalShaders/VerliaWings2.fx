sampler maskSampler : register(s0);
sampler perlinNoiseSampler : register(s2);
float2 scrollOffset;
float2 maskSize;
float2 perlinNoiseSize;
float2 tiling;

float distortionStrength;
float frequency;

float2 SampleCorrectedCoords(float2 coords, float2 sourceSize, float2 noiseSize)
{
    //No source rect needed here cause of what we're using this shader for
    //Kinda lazy i know, but this shader is really only for htis boss
    float2 correctedCoords = (coords * sourceSize) / noiseSize;
    return correctedCoords;
}


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 perlinNoiseCoords = SampleCorrectedCoords(coords, maskSize, perlinNoiseSize);
    perlinNoiseCoords += scrollOffset * 0.5;
    perlinNoiseCoords = frac(perlinNoiseCoords);
    float noise = tex2D(perlinNoiseSampler, perlinNoiseCoords);
    float2 distortionOffset = float2(0.0, 1.0) * distortionStrength 
    * lerp(-1.0, 1.0, sin(noise * frequency) * 0.5 + 0.5);
    
    //Sample mask and bloom and apply
    float2 maskUV = coords + distortionOffset;
    float4 maskColor = tex2D(maskSampler, maskUV);
    return maskColor * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};