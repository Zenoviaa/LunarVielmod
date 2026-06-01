sampler uImage0 : register(s0);
float time;

float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 diff = (coords - float2(0.5, 0.5));
    float dist = length(diff);
    float interpolant = saturate(dist / 0.4);
    float fade = ((dist - 0.4) / 0.1);
    fade = 1.0 - fade;
    

    float alpha = time / dist;
    float w = lerp(1.0, 13.0, alpha);
    alpha = 1.0 - alpha;
    
 
    float3 innerColor = lerp(float3(1.0, 1.0, 1.0), sampleColor.rgb, (1.0 - alpha));
    float3 color = lerp(sampleColor.rgb, innerColor, interpolant);
    float4 finalColor = float4(color, 1.0) * sampleColor.a * sin(pow(interpolant, w)) * fade * alpha;
    return finalColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}