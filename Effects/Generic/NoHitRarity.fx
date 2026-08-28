sampler uImage0 : register(s0);
sampler noiseSampler : register(s1);
float time;
float strength;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 noiseCoords = frac(coords + float2(0.0, time * -0.05));
    float noise = tex2D(noiseSampler, noiseCoords * 0.4).r;
    float4 spriteColor = tex2D(uImage0, coords) * tintColor - noise * 0.3;
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}