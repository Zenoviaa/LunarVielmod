sampler uImage0 : register(s0);
float3 startColor;
float3 endColor;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float3 gradientColor = lerp(startColor, endColor, coords.y);
    float4 finalColor = sampleColor;
    finalColor *= pow(1.0 - coords.y, 3.0);
    finalColor.rgb *= gradientColor;
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};