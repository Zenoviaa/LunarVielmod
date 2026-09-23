sampler spriteSampler : register(s0);
float time;
float2 texelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 baseColor = tex2D(spriteSampler, coords);
    float numSamples = 36.0;
    float diff = 0.0;

    for (float x = -1.0; x <= 1.0; x++)
    {
        for (float y = -1.0; y <= 1.0; y++)
        {
            float2 offset = float2(x, y);
            offset *= texelSize;
            float2 offsetCoords = coords + offset;
            float4 spriteColor = tex2D(spriteSampler, offsetCoords);
            

            diff += abs(spriteColor.r - baseColor.r) * spriteColor.a;
            diff += abs(spriteColor.g - baseColor.g) * spriteColor.a;
            diff += abs(spriteColor.b - baseColor.b) * spriteColor.a;
        //    diff += abs(spriteColor.a - baseColor.a);
        }

    }
    diff /= numSamples;
    if (diff > 0.004)
    {
        return tintColor ;
    } 
    else
    {
        return baseColor;
    }
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}