sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);


//Between 0-1
float time;

//How much it's gonna push out by
float strength;

//Colors!!! Woas...
float3 innerColor;
float3 bloomColor;

sampler ditherSampler : register(s2);
float2 spriteSize;
float2 ditherTexelSize;
float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * spriteSize * ditherTexelSize;
    float dither = tex2D(ditherSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float t = time;
    t += sampleColor.r * 16.0;
    float2 noiseCoords = frac(coords + float2(t * -0.05, t * -0.025));
    float noise = tex2D(noiseSampler, noiseCoords).r * 1.0;
    
    float2 noiseCoords2 = frac(coords + float2(t * 0.05, t * -0.025 + 0.3));
    float noise2 = tex2D(noiseSampler, noiseCoords2).r * 1.0;
    
    float combinedNoise = noise + noise2;

    float2 diff = coords - float2(0.5, 0.5);
    float edgeFade = length(diff) / 0.5;
    edgeFade = 1.0 - saturate(edgeFade);
    
    float3 color = lerp(innerColor, bloomColor, combinedNoise - 0.35) * (combinedNoise);
    float dither = Dither(frac(coords + float2(0.0, sin(t) * 0.001)));
    color -= dither;
    color = floor(color * 8.0) / 8.0;
    

    return float4(color, sampleColor.a) * edgeFade * 2.2 * sampleColor.b;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}