sampler spriteSampler : register(s0);
float4 startGradientColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
float4 endGradientColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    return lerp(startGradientColor, endGradientColor, coords.x) * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}