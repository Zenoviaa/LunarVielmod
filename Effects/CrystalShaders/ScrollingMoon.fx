sampler maskSampler : register(s0);
sampler scrollingMoonSampler : register(s1);
float2 scrollOffset;
float2 imageSize;
float2 maskSize;
float2 tiling;
float bendStrength;
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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 moonCoords = SampleCorrectedCoords(coords, imageSize, maskSize);

    float bump = QuadraticBump(coords.x);
    
    //Stretch the coordinates based on whether it is closer to the left or right of the texture
    float2 center = float2(0.5, 0.5);
    float2 diff = center - coords;
    float dist = length(diff);
    float maxDist = length(center);
    float bend = dist / maxDist;
    
    moonCoords -= diff * bend * bendStrength;
    moonCoords *= tiling;
    moonCoords += scrollOffset;
    moonCoords = frac(moonCoords);
    float4 moonColor = tex2D(scrollingMoonSampler, moonCoords);
    float4 maskColor = tex2D(maskSampler, coords);
  
    //Combine mask and scrolling texture color
    float4 finalColor = maskColor * moonColor * sampleColor;
    return finalColor;
//    return backgroundColor;

}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};