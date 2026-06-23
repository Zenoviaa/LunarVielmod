sampler spriteSampler : register(s0);
sampler frostTextureSampler : register(s1);
float2 frostedTexelSize;
float2 spriteSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 frostedCoords = coords * spriteSize * frostedTexelSize;
    float4 frostedColor = tex2D(frostTextureSampler, frostedCoords);
    float4 spriteColor = tex2D(spriteSampler, coords);
    
    float2 diff = coords - float2(0.5, 0.5);
    float l = saturate(length(diff) / 0.5);
    float d = frostedColor.r < l;
    frostedColor *= l;
    frostedColor *= d;
    frostedColor *= spriteColor.a;
    frostedColor *= sampleColor;
    return frostedColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};