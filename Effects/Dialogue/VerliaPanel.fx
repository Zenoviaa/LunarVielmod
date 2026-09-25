sampler spriteSampler : register(s0);
sampler starsSampler : register(s1);
float time;
float4 backSmokeColor;
float4 frontSmokeColor;

float2 DistortionOffset(float2 coords)
{
    float2 offsetCoords = coords + float2(time * 0.01, 0.4);
    offsetCoords = frac(offsetCoords);
    float n = tex2D(spriteSampler, offsetCoords).r;
    n *= 6.28;
    return (sin(n), cos(n));
}

float4 SampleStars( float2 coords)
{
    coords *= 4.2;
    float2 offsetCoords = coords + float2(time * -0.01, 0.0);
    offsetCoords = frac(offsetCoords);
    float n = tex2D(starsSampler, offsetCoords).r;
    float l = length(coords);
    float distortingNoise = tex2D(spriteSampler, frac((coords * sin(l * 50.0)) + float2(time * -0.03, time * -0.015))).r;
    n *= lerp(0, 4.3, distortingNoise);
    return (n, n, n, n);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 originalCoords = coords;
    float2 distortion = DistortionOffset(coords);
    coords += distortion * 0.3 * sin(coords.x * 6.28);
    coords.y += sin(time + coords.x * 14.0) * 0.01;
    float2 offsetCoords = coords + float2(time * -0.05, 0.0);
    offsetCoords = frac(offsetCoords);
    float n = tex2D(spriteSampler, offsetCoords).r;
    
    float2 offsetCoords2 = coords + float2(time * 0.03, 0.2);
    offsetCoords2 = frac(offsetCoords2);
    float n2 = tex2D(spriteSampler, offsetCoords2).r;
    
    float4 smokeColor = lerp(backSmokeColor, frontSmokeColor, n) ;
    float4 smokeColor2 = lerp(backSmokeColor, frontSmokeColor, n2);
    float4 smokeColor3 = float4(0.2, 0.3, 0.8, 1.0);
    float4 smokeColor4 = tex2D(spriteSampler, originalCoords);
    smokeColor3 *= coords.x;
    smokeColor3 *= cos(time + coords.x * 4.0) * 0.5 + 0.5;
    float4 stars = SampleStars(coords);
    float4 stars2 = SampleStars(originalCoords);
    stars2 *= originalCoords.x;
    float4 lowColor = max(smokeColor, smokeColor2);
    float4 highColor = smokeColor + smokeColor2;
    highColor *= coords.x;
    highColor *= 2.5;
    float offset = stars.r * 32.0;
    float4 mixedSmokeColor = lerp(lowColor, highColor, sin(time + offset) * 0.5 + 0.5);
    float4 finalColor = mixedSmokeColor;
    finalColor += smokeColor3;
    finalColor.b += 0.5;
    finalColor.rgb *= 0.35;
    finalColor += stars * 0.5 + stars2 * (cos(time + offset) * 0.5 + 0.5);

  //  finalColor -= smokeColor4 * 0.2;
    return finalColor;
  
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}