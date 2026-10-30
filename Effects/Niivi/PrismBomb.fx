#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s2);
float4 outsideColor;
float4 insideColor;
float4 outlineColor;
float2 metaballs[32];
float2 spriteSize;
float2 noiseTexelSize;
float metaballRadius;
float metaballStrength;
float time;

float DistanceSquared(float2 a, float2 b)
{
    float num = a.x - b.x;
    float num2 = a.y - b.y;
    return num * num + num2 * num2;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 ditherCoords = coords * spriteSize * noiseTexelSize;
    float4 ditherColor = tex2D(noiseSampler, ditherCoords);
  
    float pixelated = 128.0;
    coords = floor(coords * pixelated) / pixelated;
    
    //we can assume the metaballs are already in texture spac
    float totalContribution = 0.0;
    float metaballRadiusSquared = metaballRadius * metaballRadius;
    for (int i = 0; i < 32; i++)
    {
        float2 mb = metaballs[i];
        float distSquared = DistanceSquared(coords, mb);
        float influence = (metaballRadius / distSquared) * metaballStrength;
        influence += sin(time * 3.14 + coords.y * 8.0) * 0.025;

        float p = pow(influence, 1.4);
        totalContribution += p;
    }

    float l = totalContribution / 4.0;
    l = saturate(l);
    
    float4 outColor = outsideColor;
    outColor.r += sin(coords.x * 4.0 + time);
    outColor.g += cos(coords.y * 4.0 + time);
    outColor.b += sin(coords.x * 8.0 + time);
    
    float4 inColor = insideColor;
    inColor.r += sin(coords.x * 3.0 + time) * 0.2;
    inColor.g += cos(coords.y * 4.0 + time) * 0.15;
    float4 metaballColor = lerp(float4(0.0, 0.0, 0.0, 0.0), outColor, l);
    float4 metaballColor2 = lerp(outColor, inColor, l);
    float4 metaballColor3 = lerp(metaballColor, metaballColor2, l);

    metaballColor3 += ditherColor * 0.3 * metaballColor.a;
    //metaballColor3 += QuadraticBump(CircleRatioFromCenter(coords));
    metaballColor3 = floor(metaballColor3 * 8.0) / 8.0;
    metaballColor3 *= (1.0 - CircleRatioFromCenter(coords));
    metaballColor3 *= 2.0;
    return metaballColor3 * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}