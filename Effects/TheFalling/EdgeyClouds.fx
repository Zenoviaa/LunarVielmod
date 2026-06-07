sampler pixelCloudSampler : register(s0);
sampler noiseSampler : register(s1);

float time;
float distortionStrength;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 noiseSampelCoords = coords;
    noiseSampelCoords += float2(time * -0.025, time * -0.025);
    noiseSampelCoords = frac(noiseSampelCoords);
    float n = tex2D(noiseSampler, noiseSampelCoords).r;
    float radians = n * 6.28;
    float2 distortionoffset = float2(cos(radians), sin(radians)) * distortionStrength;
    
    float2 sampleCoords = coords;
    sampleCoords.y -= time * -0.05;
    sampleCoords += distortionoffset;
    sampleCoords = frac(sampleCoords);
    float4 col = tex2D(pixelCloudSampler, sampleCoords);
    col *= coords.y;
    //  col *= 1.0 - sin(length(coords - float2(0.5, 0.5)) * 3.14);
    return col * sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}