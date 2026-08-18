sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float3 bloomColor;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 inputColor : COLOR0) : COLOR0
{
    float2 offsetCoords = coords + float2(time * -0.05, 0.0);
    offsetCoords = frac(offsetCoords);
    offsetCoords.y += sin(coords.x * 8.0 + time * 0.5) * 0.3;
    float4 color = tex2D(spriteSampler, offsetCoords) * inputColor;
    float y = abs(coords.y - 0.5) / 0.5;
    float x = abs(coords.x - 0.5) / 0.5;
    color.rgb *= 1.0 - y;
    color.rgb *= bloomColor;

    color.rgb *= 2.0;

    
    float4 detailColor = tex2D(noiseSampler, frac(offsetCoords * 3.0 + float2(time * 0.5, 0.0)));
    detailColor.a = 0.0;
    color += detailColor * 0.2 * inputColor;
    color.rgb *= lerp(1.0, 0.0, x);
    return color * 1.5;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}