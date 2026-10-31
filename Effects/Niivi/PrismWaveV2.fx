#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler gradientSampler : register(s2);
sampler ditherSampler : register(s3);
float2 spriteSize;
float2 ditherTexelSize;
float2 centerPoint;
float2 centerPoint2;
float time;

Texture3D ColorSpectrumTexture;
sampler3D ColorSpectrumTextureSampler = sampler_state
{
    Texture = <ColorSpectrumTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

float4 SampleNoiseColor(float2 coords)
{
    float speed = 0.2;
    float2 noiseCoords = coords * 3.3;
    noiseCoords.x *= 2.0;
    //coords = Rotate(coords, CircleRatioFromCenter(coords) * 150.0);
    float t = time * speed;
    float2 scrollingCoords = noiseCoords + float2(t, -t);
    float2 scrollingCoords2 = noiseCoords + float2(-t + 0.3, t + 0.3);
    
    scrollingCoords.y += t * 8.0;
    scrollingCoords2.y += t * 8.0;
    float4 noiseColor1 = tex2D(noiseSampler, scrollingCoords);
    float4 noiseColor2 = tex2D(noiseSampler, scrollingCoords2);
    float4 mixedNoise = noiseColor1 + noiseColor2;
    mixedNoise /= 2.0;
    mixedNoise = floor(mixedNoise * 8.0) / 8.0;
    mixedNoise *= 1.5;
    return mixedNoise;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0, float4 pos : SV_Position) : COLOR0
{
    float4 noiseColor = SampleNoiseColor(coords);
        
    float2 ditherCoords = coords * spriteSize * ditherTexelSize;
        
    float4 ditherColor = tex2D(ditherSampler, ditherCoords);
  
    float pixelated = 1024.0;
    coords = floor(coords * pixelated) / pixelated;

    float2 originalCoords = coords;

    float4 col = float4(1.0, 0.0, 0.0, 1.0);
    col.rgb = ApplyHue(col.rgb, time * 4.0 + coords.x * 8.0);
    col.rgb = ApplyHue(col.rgb,  coords.y * 6.0);
    
   // coords.x += sin(coords.y * 32.0 + time * 32.0) * 0.003;
    

    float dist1 = abs(coords.x - centerPoint.x);
    float dist2 = abs(coords.x - centerPoint2.x);
    float dist3 = abs(coords.y - 0.95) + sin(coords.x * 8.0 + time * 12.0) * 0.05 - 0.1;
    float dist4 = abs(coords.y - 0.95) - 0.05;
    float closestDist = min(dist1, dist2);
    float d = 0.03;
  //  d += sin(time * coords.x) * 0.01;
    d += noiseColor.r * 0.03;
    float div = d / closestDist;
    float div2 = d / dist3;
    float div3 = d / dist4;
    col *= (div + div2 + div3);

    //col *= smoothstep(0.0, 1.0, coords.y);


//    col += sin(coords.y * 16.0 + time * 8.0) * col.r * 0.4;


    col += d / closestDist * 0.3;

    col -= noiseColor * col.r * 0.6;
    col *= QuadraticBump(coords.x);
        
    
    noiseColor *= 0.5;
    col *= coords.y;
    float fadeBottom = saturate((originalCoords.y - 0.9) / 0.1);
    fadeBottom = 1.0 - fadeBottom;

    
    col -= ditherColor * 0.5;
    col += QuadraticBump(d) * 0.3;
    col.rgb += ApplyHue(float3(originalCoords.y, 0.0, 0.0) * 2.0, originalCoords.y * 8.0);
    col = floor(col * 4.0) / 4.0;
    col += div * 0.1;

    
    float4 gradientColor = float4(1.0, 0.0, 0.0, 1.0);
    gradientColor.rgb = ApplyHue(gradientColor.rgb, 
    originalCoords.y + time + 4.0);
    gradientColor *= originalCoords.y;
    gradientColor *= QuadraticBump(originalCoords.x);
    //col.rgb = ApplyHue(col.rgb, time * 4.0);
    return col * tintColor  * fadeBottom  * noiseColor + gradientColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}