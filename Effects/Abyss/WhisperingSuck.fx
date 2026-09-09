
sampler spriteSampler : register(s0);
float Time;
float4 bloomColor;

float QuadraticBump(float time)
{
    return time * (4.0 - time * 4.0);
}

float4 PixelShaderFunction(float2 uv : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 coords = uv;
    coords += float2(Time, 0.0);
    coords.y += sin(Time * 0.05 + uv.x * 4.0) * 0.2;
    coords = frac(coords);
    float4 color = tex2D(spriteSampler, coords);
    color += QuadraticBump(coords.y) * bloomColor * color;
    color *= QuadraticBump(uv.x);
    return color * sampleColor * 1.5;
}

technique Technique1
{
    pass Pass0
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}