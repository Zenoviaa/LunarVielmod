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
    float2 noiseCoords = frac(coords + float2(time * -0.05, time * -0.025));
    float noise = tex2D(noiseSampler, noiseCoords).r * 0.5;
    
    float2 noiseCoords2 = frac(coords + float2(time * 0.05, time * -0.025));
    float noise2 = tex2D(noiseSampler, noiseCoords2).r * 0.5;
    
    float combinedNoise = noise + noise2;
   
    float a = combinedNoise > 0.75;
    
    float3 color = lerp(innerColor, bloomColor, combinedNoise);
    return float4(color, 1.0) * sampleColor * a;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}