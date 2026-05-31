sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float time;
float3 lightColor;
float3 darkColor;
float2 resolution;
float2 primaryTextureSize;
float2 screenOffset;
float4 SampleStars(float2 coords, in float4 dir)
{

    //Channel is between 0 and 1 so we have to multiply by two pi to get the proper angle
    float angle = dir.r * 6.28;
    float2 e = float2(cos(angle), sin(angle)) * dir.a * 0.1;
    coords += e;
    coords = frac(coords);
    float l = length(coords);
    float2 starNoiseCoords = (coords * resolution - uSourceRect.xy) / primaryTextureSize;
    
    float2 offset = uImageOffset;
    offset += coords * coords;
 
    
  
    float starNoise = tex2D(uImage3, frac(starNoiseCoords + offset)).r;
    float2 distortingCoords = (coords * sin(l * 50.0)) + float2(time * -0.03, time * -0.015) + offset;
    distortingCoords = frac(distortingCoords);
    float distortingNoise = tex2D(uImage1, distortingCoords).r;
    distortingNoise = pow(distortingNoise, 2.0);
    starNoise *= lerp(0, 1.4, distortingNoise);
    
    float4 finalColor = float4(starNoise, starNoise, starNoise, 0.0) * 0.85;
    return finalColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Basically we're going to pass in a texture that tells which direction to blow the swirls in
    //So it'll look really cool when scrolling the noise instead of just a general texture
   
    coords += screenOffset;
    coords = frac(coords);
    
    float4 dir = tex2D(uImage2, coords);
    float2 tiledCoords = coords * float2(2.0, 2.0);
 
    //Channel is between 0 and 1 so we have to multiply by two pi to get the proper angle
    float angle = dir.r * 6.28;
    float2 offset = float2(cos(angle), sin(angle)) * dir.a * 0.2;
    
   
    float2 scrollingCoords = tiledCoords + offset + float2(time * -0.05, time * -0.05);
    scrollingCoords = frac(scrollingCoords);
    float scrollingNoise = tex2D(uImage1, scrollingCoords);
    
    //TWOPI approximation is more than enough, doesn't need to be exact
    float distortionAngle = scrollingNoise * 6.28;
    float2 distortionOffset = float2(cos(distortionAngle), sin(distortionAngle)) * 0.05;
    float2 offsetCoords = tiledCoords + distortionOffset;
    offsetCoords = frac(offsetCoords);
    float noiseColor = tex2D(uImage0, offsetCoords);
    
    //Now we have the movement so we can do color grading
    float3 color = lerp(darkColor, lightColor, noiseColor);
    color = lerp(float3(0.0, 0.0, 0.0), color, scrollingNoise);
    color.rgb += SampleStars(coords, dir);
    float4 finalColor = float4(color, 1.0) * sampleColor;
    return finalColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}