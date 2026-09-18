sampler spriteSampler : register(s0);
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords);
    float len = length(coords - float2(0.5, 0.5));
    float l = saturate(len / 0.5);
    spriteColor.rgb *= lerp(float3(1.0, 1.0, 1.0), tintColor.rgb, l);
    spriteColor.rgb *= lerp(float3(1.0, 1.0, 1.0), tintColor.rgb, 0.25);
    return spriteColor * tintColor.a;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}