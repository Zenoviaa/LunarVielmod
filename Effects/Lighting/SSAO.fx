sampler heightMapSampler : register(s0);
float2 stepSize;
float2 offsets[16];

float GetAlpha(float2 coords, float2 texelOffset)
{
    const float STEPS = 5;
    float alpha = 0.0;
    for (int i = 1; i < STEPS; i++)
    {
        float2 sampleCoords = coords + texelOffset * i;
        float a = tex2D(heightMapSampler, sampleCoords).a;
        float dir = lerp(-3.0, 1.0, a);
        alpha += dir / STEPS;
    }
    return alpha;

}

float GetOcclusionFactor(float2 coords)
{
    float leftAlpha = GetAlpha(coords, float2(-stepSize.x, 0.0));
    float rightAlpha = GetAlpha(coords, float2(stepSize.x, 0.0));
    float topAlpha = GetAlpha(coords, float2(0.0, -stepSize.y));
    float bottomAlpha = GetAlpha(coords, float2(0.0, stepSize.y));
    float combinedAlpha = leftAlpha + rightAlpha + topAlpha + bottomAlpha;
    combinedAlpha /= 4.0;
    return combinedAlpha;
}

float GetOcclusionFactorV2(float2 coords)
{
    const int STEPS = 16;
    float alpha = 0.0;
    for (int i = 0; i < STEPS; i++)
    {
        float2 offset = offsets[i];
        float2 sampleCoords = coords + offset * stepSize;
        float a = tex2D(heightMapSampler, sampleCoords).a;
        float dir = lerp(-1.0, 1.0, a);
        alpha += dir / STEPS;
    }
    return alpha;
}

float GetOcclusionFactorV3(float2 coords)
{
    const int STEPS = 2;
    const float STEP_DIVISOR = 16;
    float alpha = 0.0;
    for (int x = -STEPS; x < STEPS; x++)
    {
        for (int y = -STEPS; y < STEPS; y++)
        {
            if(x == 0.0 && y == 0.0)
                continue;
            
            float2 offset = float2(x, y);
            float2 stepOffset = stepSize * offset;
            float2 sampleCoords = coords + stepOffset;
            float a = tex2D(heightMapSampler, sampleCoords).a;
            float dir = lerp(-1.0, 1.0, a);
            alpha += dir / STEP_DIVISOR;
        }
    }
    return alpha;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Naive SSAO approach, curious how this looks
    //We'll input the tile render target and draw an overlay over the screen
    float occlusion = GetOcclusionFactorV3(coords);
    return float4(0.0, 0.0, 0.0, occlusion) * sampleColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}