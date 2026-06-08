sampler uImage0 : register(s0);
float time;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //This will wrap the coords around like a sphere
    float2 uv = 1.2 * coords - 0.6;
    uv.x = acos(uv.x / cos(uv.y = asin(uv.y))) - time * 0.15;
    //uv.y -= 0.5;
    float2 scrollingCoords = uv;
    float4 bandColor = tex2D(uImage0, frac(scrollingCoords));
    
    float2 diff = (coords - float2(0.5, 0.5));
    float len = length(diff);
    bandColor.rgb *= smoothstep(1.0, 0.5, pow(saturate(len / 0.5), 3.0));
    bandColor.rgb *= sin(time + coords.x * 4.0);
    return bandColor * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};