
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 diff = coords - float2(0.5, 0.5);
    float len = length(diff);
    float interpolant = len / 0.5;
    interpolant = saturate(interpolant);
    float4 color = lerp(float4(0.0, 0.0, 0.0, 1.0), float4(0.0, 0.0, 0.0, 0.0), smoothstep(0.0, 1.0, pow(interpolant, 1.5)));
    return color * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};