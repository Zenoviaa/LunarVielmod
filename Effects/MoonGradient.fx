sampler spriteSampler : register(s0);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float a = lerp(1.0, tintColor.a, coords.y);
    return float4(a, a, a, a);
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};