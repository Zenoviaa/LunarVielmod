#include "../Helpers/Math.fxh"
sampler2D spriteSampler : register(s0);
sampler2D maskSampler : register(s1);
sampler2D noiseSampler : register(s2);


sampler2D starsSampler : register(s3);
float2 screenFixer;
float2 starsTexelSize;
float2 spriteSize;
float time;
float distortionStrength;


float4 Stars(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    coords += screenFixer;
    coords *= 0.6;

    coords = frac(coords);
    float l = length(coords);
    float2 starNoiseCoords = coords * spriteSize * starsTexelSize;
    starNoiseCoords += float2(time * -0.05, time * -0.05);
    starNoiseCoords = frac(starNoiseCoords);
    float starNoise = tex2D(starsSampler, starNoiseCoords).r;
    float distortingNoise = tex2D(noiseSampler, frac((coords * sin(l * 50.0)) + float2(time * -0.03, time * -0.015))).r;
    starNoise *= lerp(-1.4, 2.4, distortingNoise);
    
    float4 finalColor = float4(starNoise, starNoise, starNoise, 0.0) * starNoise;
    return finalColor;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 originalColor = tex2D(spriteSampler, coords);
    float2 maskCoords = coords;
    maskCoords.x += sin(coords.y * 64.0 + time * 4.0) * 0.001;
    float4 maskColor = tex2D(maskSampler, maskCoords);
    if(maskColor.r <= 0.0)
        return originalColor;
    
    float strengthAtPoint = lerp(1.0, 0.0, maskColor.r);
    strengthAtPoint *= distortionStrength;
    float2 scrollingCoords = frac(coords + float2(time * -0.05, time * -0.025) + screenFixer);
    float noise = tex2D(noiseSampler, scrollingCoords).r;
    float2 distortionOffset = float2(cos(noise * 3.14), sin(noise * 3.14)) * strengthAtPoint * noise * (1.0 - maskColor.r);
    
    
    float2 scrollingCoords2 = frac(coords + float2(0.0, time * -0.4));
    float noise3 = tex2D(noiseSampler, scrollingCoords2).r;
    distortionOffset.y += noise3 * 0.;
    
    
    
    float4 distortedColor = tex2D(spriteSampler, coords + distortionOffset);
    float4 finalTint = lerp(float4(1.0, 1.0, 1.0, 1.0), tintColor, 1.0 - maskColor.r);
    distortedColor *= tintColor;
    
    float r = maskColor.r;

    float b = saturate(r / 0.25);
    float4 ringColor = lerp(float4(1.0, 1.0, 1.0, 1.0), tintColor, QuadraticBump(b));
    ringColor.r += sin(time);
    ringColor.g += cos(time);
    ringColor *= 3.0;
    distortedColor.r = max(distortedColor.g, max(distortedColor.b, distortedColor.r));
    distortedColor.gb = distortedColor.r;
    distortedColor += ringColor * QuadraticBump(b) * 2.5 * sin(coords.y * 8.0 + time) * 0.03;
    
    float noise2 = tex2D(noiseSampler, frac(scrollingCoords * 4.0)).r;
    noise2 = pow(noise2, 3.0);
    distortedColor += noise2 * 0.3;
    distortedColor = floor(distortedColor * 8.0) / 8.0;
    distortedColor = lerp(distortedColor, tintColor, 0.14);

    distortedColor.b += 0.15;
    distortedColor += Stars(coords) * 0.4;

    float4 finalColor = lerp(originalColor, distortedColor, pow(maskColor.r, 0.2) * 0.8);
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}