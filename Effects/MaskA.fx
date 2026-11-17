sampler spriteToDraw : register(s0);
texture maskTexture;
sampler2D maskTex = sampler_state
{
    texture = <maskTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 mask = tex2D(maskTex, coords);
    float a = mask.a > 0.0;
    float4 color = tex2D(spriteToDraw, coords);
    return color * a * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}