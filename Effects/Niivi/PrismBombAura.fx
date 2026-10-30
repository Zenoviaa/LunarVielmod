#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float4 outsideColor;
float4 insideColor;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float pixelated = 256.0;
    coords = floor(coords * pixelated) / pixelated;
    
    //For this effect, I want to use polar coordinates to scroll a texture outward and have it lerp between the insideColor and outside Color
    //While slowly going transparent and distorting along that path
    //It should also swirl a bit, so use that rotation technique, I think I remember how to do that?
    float ratioFromCenter = CircleRatioFromCenter(coords);
    float2 circleCoords = RotateAround(coords, float2(0.5, 0.5), ratioFromCenter * 3.5);
    float2 pc = PolarCoordinates(circleCoords);
    pc.y += time * 1.2;
    pc.x += time * 0.2;
    
    float4 noiseColor = tex2D(spriteSampler, pc);
    
    float4 outColor = outsideColor;
    outColor.r += sin(coords.x * 4.0 + time);
    outColor.g += cos(coords.y * 4.0 + time);
    outColor.b += sin(coords.x * 8.0 + time);
    
    float4 glowingLerp = lerp(insideColor, outColor, noiseColor.a);
    glowingLerp *= smoothstep(1.0, 0.0, CircleRatioFromCenter(coords));
    glowingLerp *= 1.0 / ratioFromCenter;
    glowingLerp += (1.0 - ratioFromCenter) * 2.0;
    glowingLerp += sin(ratioFromCenter * 64.0 + time * -12.0) * 0.15;
    glowingLerp = floor(glowingLerp * 8.0) / 8.0;
 
    return glowingLerp * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}