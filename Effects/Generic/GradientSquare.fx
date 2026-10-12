

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float4 startGradientColor;
float4 endGradientColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(noiseSampler, frac(coords + float2(time * -0.05, 0.0)));
    float4 gradientColor = lerp(startGradientColor, endGradientColor, coords.x);
    return gradientColor + spriteColor * 0.05;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
