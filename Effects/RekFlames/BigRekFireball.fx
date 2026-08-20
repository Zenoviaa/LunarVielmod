sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);

//Between 0-1
float time;

//How much it's gonna push out by
float strength;

//Colors!!! Woas...
float3 innerColor;
float3 bloomColor;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Distorting Big Rek Balls
    float2 distortionNoiseCoords = coords + float2(time * -0.03, 0.3);
    distortionNoiseCoords = frac(distortionNoiseCoords);
    float distortingNoise = tex2D(noiseSampler, distortionNoiseCoords).r;
    
    float2 noiseCoords = coords + float2(time * -0.05, 0.0);
    noiseCoords = frac(noiseCoords);
    float tiling = lerp(0.3, 4.0, coords.x);
    
    //Hope this works?
    noiseCoords *= tiling;
    noiseCoords.y += sin(distortingNoise * 3.14) * strength;
    float noise = tex2D(noiseSampler, noiseCoords).r;
    float3 particleColor = lerp(innerColor, bloomColor, noise);
    
    float fade = smoothstep(0.0, 1.0, coords.x);
    float4 finalcolor = float4(particleColor, 1.0) * sampleColor * fade;
    return finalcolor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}