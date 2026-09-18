sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    l += time;
    l += sampleColor.a * 32.0;
    l = frac(l);
    float s = sin(l * 6.28);
    float4 spriteColor = tex2D(spriteSampler, coords);
    spriteColor *= s;
    spriteColor.rgb *= sampleColor.rgb;
    spriteColor.a = 0.0;
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}