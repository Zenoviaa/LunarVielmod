sampler uImage0 : register(s0);
float2 particles[32];
float4 insideColor;
float4 bloomColor;
float particleRadius;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    
    //Get the particle color at this coordinate
    //We're oging to draw this particle system in screen space
    float contribution = 0.0;
    float falloff = 1.0;
    float falloffFactor = 1.0 / 32.0;
    for (int i = 0; i < 32; i++)
    {
        float2 particle = particles[i];
        float dist = distance(particle, coords);
        
        float radius = particleRadius ;
        float particleContribution = 1.0 - saturate(dist / radius);
        contribution += particleContribution * falloff;
        falloff -= falloffFactor;
    }
    
    //Calculate the particle color based on the contribution
    float4 iColor = lerp(insideColor, float4(1.0, 1.0, 1.0, 1.0), contribution * 0.25);

    float4 particleColor = lerp(bloomColor, iColor, contribution) * pow(contribution, 0.75);
    particleColor *= sampleColor;

    return particleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}