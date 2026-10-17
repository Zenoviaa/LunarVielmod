sampler spriteSampler : register(s0);
float2 texelSize;

//Ported this shader: https://www.shadertoy.com/view/4tSyzy
//The output should probably be exported to a render target
float2 pow2(float p)
{
    return pow(p, 2.0);
}

float gaussian(float2 i)
{
    float pi = 3.14;
    const int maxSamples = 4;
    float sigma = float(maxSamples) * 0.25;
    return 1.0 / (2.0 * pi * pow2(sigma)) * exp(-((pow2(i.x) + pow2(i.y)) / (2.0 * pow2(sigma))));
}

float3 blur(sampler2D sp, float2 uv, float2 scale)
{
    const int maxSamples = 4;
    float3 col = float3(0.0, 0.0, 0.0);
    float accum = 0.0;
    float weight;
    float2 offset;

    for (int x = -maxSamples / 2; x < maxSamples / 2; ++x)
    {
        for (int y = -maxSamples / 2; y < maxSamples / 2; ++y)
        {
            offset = float2(x, y);
            weight = gaussian(offset);
            col += tex2D(sp, uv + scale * offset).rgb * weight;
            accum += weight;
        }
    }
    
    return col / accum;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 color;
    color.rgb = blur(spriteSampler, coords, texelSize);
    color.a = 1.0;
    return color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}