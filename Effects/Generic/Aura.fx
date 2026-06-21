sampler uImage0 : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    uv += float2(time * -0.05, time * 0.05);
    uv = frac(uv);
    
    float dist = distance(coords, float2(0.5, 0.5));
    float alpha = 1.0f - saturate(dist / 0.5);
    float4 color = tex2D(uImage0, uv);
    color *= alpha;
    color *= tintColor;
    return color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}