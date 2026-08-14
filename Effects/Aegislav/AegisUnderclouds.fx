sampler uImage0 : register(s0);
sampler cloudDetailSampler : register(s1);
float time;
float distortionStrength;
float2 detailSize;
float2 spriteSize;
float2 resolution;

float2 SampleCoordinates(float2 coords, float2 offset)
{
    float curveFactor = 3.5;
    float2 offsetPosition = float2(0.5, 0.0);
    float2 bentCoords = float2(coords.x + offsetPosition.x * 0.5 / resolution.x, coords.y - pow(coords.x - 0.5 - offsetPosition.x / resolution.x, 2) * (coords.y - 0.5 - offsetPosition.y / resolution.y) * curveFactor);
    float roll = sin(time * 6.28 + bentCoords.x * 3);
    float2 uv2 = bentCoords + resolution * 1.2 + float2(time * 3, roll * 0.1 + offsetPosition.y / resolution.y * 0.5);
    uv2 *= 4.8;
    uv2 += offset;
    return uv2;
}

float3 SampleNormalMap(in float2 coords)
{

    float2 offsetCoords = coords + float2(time * -0.015, 0.5);
    offsetCoords = frac(offsetCoords);
    //Sample the second half of the texture, that's where hte normal map is
    //offsetCoords.y *= 0.5 + 0.5;

    float3 normalSample = tex2D(uImage0, offsetCoords).rgb;
    
    //Range -1 to 1
    float3 normalVec = normalSample * 2.0 - 1.0;
    return normalVec;
}

float4 SampleCloudDetails(in float2 coords)
{
    float2 noiseCoords = (coords * spriteSize * 1.5) / detailSize;
    noiseCoords = frac(noiseCoords);
   // coords *= 2.0;
    float2 cloudCoords = noiseCoords + float2(time * -0.025, 0.0);
    cloudCoords = frac(cloudCoords);
    float4 details = tex2D(cloudDetailSampler, cloudCoords) * 0.5;
    
    float2 cloudCoords2 = noiseCoords + float2(time * 0.025, 0.4);
    cloudCoords2 = frac(cloudCoords2);
    float4 details2 = tex2D(cloudDetailSampler, cloudCoords2) * 0.5;
    
    float4 mixedDetails = details + details2;
    mixedDetails.rgb = lerp(float3(1.0, 0.4, 0.4), float3(0.4, 0.0, 0.0), mixedDetails.r);
    float a = mixedDetails.r > 0.25;
    return mixedDetails * a;
}

float4 SampleClouds(in float2 coords, in float2 normalCoords)
{
    float2 cloudCoords = coords + float2(time * -0.025, 0.0);
    cloudCoords = frac(cloudCoords);
    cloudCoords.y *= 0.5;
    float2 nCoords = coords;
    nCoords.y *= 0.5;
    float3 normalVec = SampleNormalMap(nCoords);

    cloudCoords.y += normalVec.y * distortionStrength;
    cloudCoords.x += normalVec.x * distortionStrength;
    float4 cloudMask = tex2D(uImage0, cloudCoords);
    
    float4 cloudDetails = SampleCloudDetails(cloudCoords);
    float4 mixedClouds = cloudMask * cloudDetails;
    return mixedClouds;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 clouds = SampleClouds(coords, coords) * sampleColor;
    clouds = floor(clouds * 8.0) / 8.0;
    return clouds;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}