#include "Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float2 centerPoint;
float2 texelSize;
float strength;
float time;



float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 screenSize = float2(1.0, 1.0) / texelSize;
    float asp = screenSize.x / screenSize.y;
    float2 fixer = float2(asp, 1.0);
    
    float circleratio = CircleRatioFromCenter(coords * fixer, centerPoint * fixer);
    float2 swirlingCoords = RotateAround(coords * fixer, centerPoint * fixer, circleratio * 0.03 * strength  );

//    swirlingCoords *= float2(1.0, 1.0) / float2(texelSize.y, texelSize.x);
    float4 swirlPixelColor = tex2D(spriteSampler, swirlingCoords);
    float4 pixelColor = tex2D(spriteSampler, coords);
    float2 direction = (centerPoint - coords);
    float2 pullDirection = direction * texelSize * strength;
    
    
    float2 distanceTo = distance(centerPoint, coords);
    
    if (length(distanceTo) < length(pullDirection))
        pullDirection = distanceTo;
    
    float2 pullCoords = coords + pullDirection;
    float4 pullColor = tex2D(spriteSampler, pullCoords);
    float highest = max(pullColor.r, max(pullColor.g, pullColor.b));
    pullColor.rgb = highest;
    pullColor.rgb *= float3(1.0, 0.0, 0.0);
    pullColor *= 4.0;
    pullColor.rgb = ApplyHue(pullColor.rgb, time * 4.0 + coords.x * 4.0 + coords.y * 4.0);
    float d = distance(coords, centerPoint);
    float a = 0.35 / d;
    float sAlpha = strength / 600.0;
    return pixelColor + pullColor * 0.35 * a   + swirlPixelColor * 0.15 * a ;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};