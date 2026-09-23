sampler spriteSampler : register(s0);
float time;
float2 texelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 offsetCoords = coords;
    offsetCoords.x += sin(time + coords.y * 12.56) * texelSize.x * 16.0;
    float4 baseColor = tex2D(spriteSampler, offsetCoords);
    return baseColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}