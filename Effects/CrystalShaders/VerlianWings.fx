sampler maskSampler : register(s0);
sampler scrollingTextureSampler : register(s1);
sampler perlinNoiseSampler : register(s2);
float2 scrollOffset;
float2 maskSize;
float2 perlinNoiseSize;
float2 scrollingTextureSize;
float2 tiling;

float distortionStrength;
float3 bloomColorStart;
float3 bloomColorEnd;
float frequency;

float2 SampleCorrectedCoords(float2 coords, float2 sourceSize, float2 noiseSize)
{
    //No source rect needed here cause of what we're using this shader for
    //Kinda lazy i know, but this shader is really only for htis boss
    float2 correctedCoords = (coords * sourceSize) / noiseSize;
    return correctedCoords;
}

float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}


float2 SampleScrollingTexture(float2 uv, float2 distortionOffset)
{
    float2 baseCoords = uv;
    baseCoords *= tiling;
    baseCoords += scrollOffset;
    baseCoords += distortionOffset;
    baseCoords = frac(baseCoords);
    return baseCoords;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 perlinNoiseCoords = SampleCorrectedCoords(coords, maskSize, perlinNoiseSize);
    perlinNoiseCoords += scrollOffset * 0.5;
    perlinNoiseCoords = frac(perlinNoiseCoords);
    float noise = tex2D(perlinNoiseSampler, perlinNoiseCoords);
    float2 distortionOffset = float2(0.0, 1.0) * distortionStrength 
    * lerp(-1.0, 1.0, sin(noise * frequency) * 0.5 + 0.5);
    
    float2 moonCoords = SampleCorrectedCoords(coords, maskSize, scrollingTextureSize);

    //Sample mask and bloom and apply
    float2 maskUV = coords + distortionOffset;
    float4 maskColor = tex2D(maskSampler, maskUV);
    
    float starColor = tex2D(scrollingTextureSampler, SampleScrollingTexture(moonCoords, distortionOffset));
    float starColor2 = tex2D(scrollingTextureSampler, SampleScrollingTexture(moonCoords, distortionOffset + float2(-0.05, 0.2)));
    float starColor3 = tex2D(scrollingTextureSampler, SampleScrollingTexture(moonCoords, distortionOffset + float2(-0.2, 0.4)));
    
    float3 bloomColor = lerp(bloomColorEnd, bloomColorStart, starColor);
    float3 bloomColor2 = lerp(bloomColorEnd, bloomColorStart, starColor2);
    float3 bloomColor3 = lerp(bloomColorEnd, bloomColorStart, starColor3);
    float3 finalBloomColor = bloomColor * 0.25 + bloomColor2 * 0.45 + bloomColor3;
    
   // bloomColor *= starColor;   
    
    float4 finalColor = maskColor * float4(finalBloomColor.r, finalBloomColor.g, finalBloomColor.b, 1.0) * sampleColor;
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};