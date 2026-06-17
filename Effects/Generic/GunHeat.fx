sampler uImage0 : register(s0);
float3 hottestColor;
float3 coldestColor;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float progress = saturate((uv.x - 0.5) / 0.5);
    float3 heatColor = lerp(coldestColor, hottestColor, pow(progress * time, 2.0));
    float4 spriteColor = tex2D(uImage0, uv);
    float3 newRGB = lerp(spriteColor.rgb, heatColor, time * progress) * spriteColor.a;
    newRGB = floor(newRGB * 4.0) / 4.0;
    spriteColor.rgb = newRGB;
    spriteColor *= tintColor;
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}