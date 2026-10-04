sampler spriteSampler : register(s0);
float contrast;
float brightness;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 pixelColor = tex2D(spriteSampler, coords);
    pixelColor.rgb /= pixelColor.a;
    pixelColor.rgb = ((pixelColor.rgb - 0.5f) * max(contrast, 0)) + 0.5f;
    pixelColor.rgb += brightness;
    pixelColor.rgb *= pixelColor.a;
    return pixelColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};