sampler spriteSampler : register(s0);
float3 particles[100];
float2 texelSize;
float InExpo(float t)
{
    const float p = 10.0;
    return t == 0 ? 0 : pow(2, p * t - p);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float2 fixer = float2(texelSize.x / texelSize.y, 1.0);
    uv *= fixer;

    //Max number of metaballs we're sampling
    float totalContribution = 0.0;
    float radius = 0.06;
    
    //Going to try drawing this in screenspace, single pass on a render target
    for (int i = 0; i < 100; i++)
    {
        float2 pos = particles[i].xy;
        float radius = particles[i].z;
        float r = distance(uv, pos * fixer);
        float contribution = lerp(1.0, 0.0, saturate(r / radius));
        totalContribution += contribution;
    }
    
    return float4(totalContribution, totalContribution, totalContribution, totalContribution);
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}