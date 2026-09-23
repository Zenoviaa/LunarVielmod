sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 offsetCoords =  coords + float2(time * 0.05, 0.0);
    offsetCoords = frac(offsetCoords);
    float4 spriteColor = tex2D(spriteSampler, offsetCoords);
    spriteColor += sin(time * 0.2 + spriteColor.r * 6.28);

    return spriteColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}