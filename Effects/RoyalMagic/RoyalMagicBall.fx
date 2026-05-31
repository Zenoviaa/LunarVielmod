sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float time;
float3 bloomColor;
float distortion;
float2 resolution;
float2 primaryTextureSize;
float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float2 polarUV = float2(angle, dist);
    return polarUV;
}

float2 SamplePushOffset(float2 coords)
{
    float2 polarCoords = PolarCoordinates(coords);
    polarCoords += float2(time * 0.05, time * 0.05);
    polarCoords = frac(polarCoords);
    
    float noise = tex2D(uImage1, polarCoords);
    float2 pushOffset = (coords - float2(0.5, 0.5));
    pushOffset *= noise * distortion;
    return pushOffset;
}
float4 SampleStars(float2 coords)
{

    //Channel is between 0 and 1 so we have to multiply by two pi to get the proper angle
    coords = frac(coords);
    float l = length(coords);
    float2 starNoiseCoords = (coords * resolution) / primaryTextureSize;
  
    float starNoise = tex2D(uImage2, frac(starNoiseCoords)).r;
    float2 distortingCoords = (coords * sin(l * 50.0)) + float2(time * -0.03, time * -0.015);
    distortingCoords = frac(distortingCoords);
    float distortingNoise = tex2D(uImage1, distortingCoords).r;
    distortingNoise = pow(distortingNoise, 2.0);
    starNoise *= lerp(0, 1.4, distortingNoise);
    
    float4 finalColor = float4(starNoise, starNoise, starNoise, 0.0) * 0.85;
    return finalColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //We're going to draw a glowball with a bunch of stars in it I think
    //and distort with some noise
    float2 pushOffset = SamplePushOffset(coords);
    float2 adjustedCoords = coords + pushOffset;
  //  adjustedCoords = frac(adjustedCoords);
    
    //calculate the base glow ball
    float interpolant = saturate(length(adjustedCoords - float2(0.5, 0.5)) / 0.4);
    interpolant = 1.0 - interpolant;
    float3 color = lerp(bloomColor, float3(1.0, 1.0, 1.0), smoothstep(0.0, 1.0, interpolant));
    color = lerp(color, float3(0.0, 0.0, 0.0), interpolant + interpolant * (sin(time * 0.15) * 0.5 + 1.0));
    float4 finalColor = float4(color, 1.0) * sampleColor * interpolant * 2.0;
    
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}