sampler spriteSampler : register(s0);
float3 particles[100];
float2 texelSize;

float DistanceSquared(float2 a, float2 b)
{
    float num = a.x - b.x;
    float num2 = a.y - b.y;
    return num * num + num2 * num2;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float2 fixer = float2(texelSize.x / texelSize.y, 1.0);
    uv *= fixer;

    //Max number of metaballs we're sampling
    float totalContribution = 0.0;
    float metaballStrength = 0.004;
    //Going to try drawing this in screenspace, single pass on a render target
    for (int i = 0; i < 100; i++)
    {
        float3 mb = particles[i];
        float distSquared = DistanceSquared(coords, mb.xy);
      
        float influence = (mb.z / distSquared) * metaballStrength;
        totalContribution += influence;
    }
    
    totalContribution = saturate(totalContribution);
    float a = totalContribution > 0.8;

    
    return float4(a, a, a, a);
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}