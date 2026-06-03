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

float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float2 rotate(float2 uv, float2 pivot, float angle)
{
    //rotation matrix
    float2x2 rotation = float2x2(
            float2(sin(angle), -cos(angle)),
			float2(cos(angle), sin(angle)));
    
    uv -= pivot;
    uv = mul(uv, rotation);
    uv += pivot;
    return uv;
}
float spiral(float2 m)
{
    float r = length(m);
    float a = atan2(m.y, m.x);
    float v = sin(100. * (sqrt(0.5) - 0.02 * a - .3 * time * -0.05));
    return clamp(v, 0., 1.);

}

float4 SampleCloudColor(float2 coords, float4 sampleColor)
{
    float2 uv = coords;
    coords.y += smoothstep(0.0, 2.2, sin(coords.x * 6.28 + time * 0.5));
    coords.y -= smoothstep(0.0, 1.0, coords.x);
    float2 noiseCoords = coords + float2(time * 0.05, time * -0.05);
    noiseCoords = frac(noiseCoords);
    float n1 = tex2D(uImage0, noiseCoords).r;
    
    float2 noiseCoords2 = coords + float2(time * -0.05, time * -0.05 + 0.2);
    noiseCoords2.x += sin(coords.x + time * 0.15) * 0.1;
    noiseCoords2 = frac(noiseCoords2);
    noiseCoords2.x = 1.0 - noiseCoords2.x;

    float n2 = tex2D(uImage0, noiseCoords2).r;
    
    float2 noiseCoords3 = coords + float2(time * -0.05, time * -0.05 + 0.35);
    noiseCoords3 *= 3.0;
    noiseCoords3 = frac(noiseCoords3);
 
    float n3 = tex2D(uImage0, noiseCoords3).r;

    float2 s2 = spiral(uv * 0.2);
    float s3 = tex2D(uImage1, s2);
    float combinedNoise = n1 + n2 + (n3 * 0.82);
    combinedNoise *= 0.5;
 //   combinedNoise *= swirl;
    
    float4 cloudColor = float4(combinedNoise, combinedNoise, combinedNoise, 1.0) * sampleColor;
    float s = sin(time) * 0.1;
    s += 0.17;
    cloudColor *= s;
    cloudColor *= smoothstep(0.0, 1.0, coords.y);
   
    
    float3 gradientCol = lerp(float3(0.5, 0.0, 0.5), float3(1.0, 1.0, 1.0), coords.y);
    cloudColor.rgb += gradientCol * 0.1;
    return cloudColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{

    float2 uv = coords;

    coords = rotate(uv, float2(0.5, 0.5), time * -0.5);
    float4 cloudColor1 = SampleCloudColor(uv, sampleColor);
    float4 cloudCOlor2 = SampleCloudColor(coords, sampleColor);
    cloudCOlor2 += float4(1.0, 1.0, 1.0, 1.0) * 0.3 * cloudCOlor2.r;
    float4 finalCloudColor = cloudColor1 + cloudCOlor2 * 0.5;
   
    return finalCloudColor;

}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}