#include "Math.fxh"

sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{  
    float2 polarCoords = PolarCoordinates(coords);
    polarCoords.x += time * -4.0;
    polarCoords.x = frac(polarCoords.x);
    float4 laserColor = tex2D(spriteSampler, polarCoords);
    laserColor *= QuadraticBump(polarCoords.y) * 1.5;
    laserColor.rgb *= tintColor.rgb;
    laserColor -= laserColor.r * tintColor.a * 2.0;
    laserColor.a = 0.0;
    float circle = 1.0 - CircleRatioFromCenter(coords);
    laserColor.rgb +=tintColor.rgb * circle * (1.0 - tintColor.a);
    return laserColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}