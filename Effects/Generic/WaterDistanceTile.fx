sampler spriteSampler : register(s0);
float2 texelSize;

float March(in float2 coords, in float2 direction, in float maxSteps)
{
    for (float f = 0.0; f < maxSteps; f++)
    {
        float a = tex2D(spriteSampler, coords + direction * texelSize * f).a;
        if(a >= 1.0)
            return f;
    }
    return 8.0;

}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    //We're operating over the tile target here
    float currentPixel = tex2D(spriteSampler, coords).a;
    
    //Only check where there are no tiles
    if(currentPixel > 0.0)
        return 0.0;
    
    float maxSteps = 8;
    float left = March(coords, float2(-1.0, 0.0), maxSteps);
    float right = March(coords, float2(1.0, 0.0), maxSteps);
    float down = March(coords, float2(0.0, 1.0), maxSteps);
    float shortestDistance = min(left, min(right, down));
    
    //Need to do an inverse lerp
    float brightness = lerp(1.0, 0.0, saturate(shortestDistance / 8.0));
    return float4(brightness, brightness, brightness, 1.0) * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}