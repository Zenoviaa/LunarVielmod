sampler pixelCloudSampler : register(s0);
sampler noiseSampler : register(s1);

float3 goldColor;
float2 parallax;
float time;
float distortionStrength;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float2 p = parallax;
    p.y *= 0.0;
    coords.x *= 2.0;
    coords += p;
    coords = frac(coords);
    
    float2 noiseSampelCoords = coords;
    noiseSampelCoords.x *= 2.0;
    noiseSampelCoords += float2(time * -0.025, time * -0.025);
    noiseSampelCoords = frac(noiseSampelCoords);
    float n = tex2D(noiseSampler, noiseSampelCoords).r;
    float radians = n * 6.28;
    float2 distortionoffset = float2(0.0, sin(radians)) * distortionStrength;
    
    float2 sampleCoords = coords;
    sampleCoords.x -= time * -0.05;
    sampleCoords = frac(sampleCoords);
    sampleCoords += distortionoffset;

    float4 col = tex2D(pixelCloudSampler, sampleCoords);
    float d = 1.0 - saturate(sampleCoords.y / 0.6);
    d = pow(d, 0.5);
    
    float r = col.r;
 //   r = pow(r, 4.0);

    col.rgb = float3(1.0, 1.0, 1.0) - col.rgb;
    col.rgb *= col.a;
    col.rgb *= 0.1;
    col.rgb += goldColor * d * r * 1.5;
    return col * sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}