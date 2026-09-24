sampler spriteSampler : register(s0);
float time;
float2 texelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 offsetCoords = coords;
    offsetCoords.x += sin(time + coords.y * 24.56) * texelSize.x * 8.0;
    float4 baseColor = tex2D(spriteSampler, offsetCoords);
    baseColor = lerp(baseColor, float4(1.0, 1.0, 0.0, baseColor.a) * baseColor.a, coords.y);
    baseColor *= coords.y ;

    return baseColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}