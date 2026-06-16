sampler uImage0 : register(s0);
float2 particles[48];
float4 bloomColor;
float4 farColor;
float4 closeColor;
float2 centerNormalCoord;
float particleRadius;
float time;
float swirliness;
float2 Rotate(float2 uv, float2 pivot, float angle)
{
    //rotation matrix
    float2x2 rotation = float2x2(
            float2(sin(angle), -cos(angle)),
			float2(cos(angle), sin(angle)));
    
    uv -= pivot;
    uv = mul(uv, rotation);
    uv += pivot;
    return uv;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 diff = coords - float2(0.5, 0.5);
    float len = length(diff);
    float distanceInterpolant = len / 0.5;
    distanceInterpolant = clamp(distanceInterpolant, 0.0, 1.0);    
    coords = Rotate(coords, float2(0.5, 0.5), time * -0.2 + distanceInterpolant * swirliness);

    float2 uv = coords;
    
    //Get the particle color at this coordinate
    //We're oging to draw this particle system in screen space
    float contribution = 0.0;
    for (int i = 0; i < 48; i++)
    {
        float2 particle = particles[i];
        float dist = distance(particle, coords);
        float particleContribution = 1.0 - saturate(dist / particleRadius);
        contribution += particleContribution;
    }
    
    //Calculate the particle color based on the contribution
    float4 particleColor = lerp(bloomColor, float4(1.0, 1.0, 1.0, 1.0), contribution) * contribution;
    
    //Tint the particle color based on how far the particle is from the center
    float distToCenter = distance(coords, centerNormalCoord);
    float distanceInterp = saturate(distToCenter / 0.5);
    float4 distanceColor = lerp(closeColor, farColor, distanceInterp);
    particleColor *= distanceColor;
    particleColor *= sampleColor;
    particleColor *= 1.5;
    particleColor *= sin(distanceInterpolant * 6.28 + 3.14);
    //particleColor.rgb = round(particleColor.rgb * 8.0) / 8.0;
    return particleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}