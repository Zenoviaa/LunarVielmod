sampler uImage0 : register(s0);
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float time = tintColor.a;
    float2 uv = coords;
    float diff = length(uv - float2(0.5, 0.5));
    float alpha = saturate(diff / 0.5);
    float fade = alpha * (1.0 - time);
    float4 spriteColor = tex2D(uImage0, coords) * fade;
    spriteColor.rgb *= tintColor.rgb;
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}