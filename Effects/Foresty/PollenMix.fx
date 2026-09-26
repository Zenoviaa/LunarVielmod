sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float4 darkColor;
float4 lightColor;
float2 spriteSize;
float2 noiseTexelSize;
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 noiseCoords = coords * spriteSize * noiseTexelSize;
    float2 noiseCoords1 = noiseCoords + float2(time * -0.05, 0.2);
    float2 noiseCoords2 = noiseCoords + float2(time * 0.2, 0.4);
    float2 noiseCoords3 = noiseCoords + float2(time * 0.4, 0.8);
    
    noiseCoords1 = frac(noiseCoords1);
    noiseCoords2 = frac(noiseCoords2);
    noiseCoords3 = frac(noiseCoords3);
    
    float distortingNoise = tex2D(noiseSampler, noiseCoords3).r;
    float2 distortionOffset = float2(cos(distortingNoise * 6.28), sin(distortingNoise * 6.28));
    distortionOffset *= noiseTexelSize * 4.0;
    
    float noise1 = tex2D(noiseSampler, noiseCoords1 + distortionOffset).r;
    float noise2 = tex2D(noiseSampler, noiseCoords2 + distortionOffset).r;
    
    float mixedNoise = max(noise1, noise2);
    float4 pollenColor = lerp(darkColor, lightColor, mixedNoise);
    
    float4 maskColor = tex2D(spriteSampler, coords);
    float4 finalColor = pollenColor * maskColor;
    return finalColor * tintColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}