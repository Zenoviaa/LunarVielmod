sampler spriteSampler : register(s0);
sampler distortionSampler : register(s1);

float time;
float strength;
float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 distortingNoiseCoords = coords + float2(0.0, time * -0.1);
    distortingNoiseCoords = frac(distortingNoiseCoords * 3.0);
    float distortionNoise = tex2D(distortionSampler, distortingNoiseCoords);
    
    float2 spriteCoords = coords + float2(time * -0.1, sin(distortionNoise) * strength);
    spriteCoords = frac(spriteCoords);
    float4 spriteColor = tex2D(spriteSampler, spriteCoords) * sampleColor;
    
    
    float2 flameNoiseCoords = coords + float2(time * -0.15, 0.3);
    flameNoiseCoords = frac(flameNoiseCoords * 3.0);
    float flameNoise = tex2D(distortionSampler, flameNoiseCoords);
    spriteColor *= pow(coords.x, 0.5);
    spriteColor *= QuadraticBump(coords.y);
    spriteColor *= QuadraticBump(coords.x);
    return spriteColor * 3.0;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}