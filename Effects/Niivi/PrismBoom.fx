#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0, float4 pos : SV_Position) : COLOR0
{
    float pixelated = 128.0;
    coords = floor(coords * pixelated) / pixelated;
    float2 polarCoords = PolarCoordinates(coords );
    polarCoords.y -= time;
    float cRatio = CircleRatioFromCenter(coords);
    float4 col = float4(1.0, 0.0, 0.0, 1.0);
    col.rgb = ApplyHue(col.rgb, polarCoords.y * 4.0  + time * 16.0);
    col *= 2.7;

    
    float4 spriteColor = tex2D(spriteSampler, polarCoords);
    //float d = lerp(1.0, 0.0, cRatio);
    //float d2 = spriteColor.r < d;
    spriteColor *= col;

    spriteColor *= lerp(1.0, 0.0, cRatio);
    spriteColor += QuadraticBump(cRatio);
//    spriteColor = floor(spriteColor * 12.0) / 12.0;
    spriteColor *= tintColor;
    spriteColor = floor(spriteColor * 4.0) / 4.0;
    //spriteColor *= lerp(1.0, 2.0, cRatio);
    //spriteColor *= d2;
    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}