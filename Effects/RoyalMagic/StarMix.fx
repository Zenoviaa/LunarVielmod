sampler uImage0 : register(s0);
texture mixTexture;
sampler2D mixTex = sampler_state
{
    texture = <mixTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float4 colorToMix = tex2D(mixTex, coords);
    float4 mask = tex2D(uImage0, coords);
    float4 mixedColor = colorToMix * sampleColor * mask;
    return mixedColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}