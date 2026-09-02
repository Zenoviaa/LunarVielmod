sampler spriteSampler : register(s0);
float2 texelSize;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(spriteSampler, coords);
    if(baseColor.a > 0)
        return baseColor;

    float a = 0.0;
    for (float x = -1.0; x <= 1.0; x++)
    {
        for (float y = -1.0; y <= 1.0; y++)
        {
            if(x == 0.0 && y == 0.0)
                continue;
            float2 offsetCoords = coords + texelSize * float2(x, y);
            a = max(a, tex2D(spriteSampler, offsetCoords).a);
        }
    }

    return sampleColor * a;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};