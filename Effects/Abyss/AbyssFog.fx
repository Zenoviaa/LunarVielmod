sampler spriteSampler : register(s0);
sampler ditherSampler : register(s1);
float time;
float2 ditherTexelSize;
float2 spriteSize;
float2 screenOffset;

float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * spriteSize * ditherTexelSize;
    float dither = tex2D(ditherSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Screen color
    float2 fogCoords = coords;
    fogCoords *= float2(0.3, 1.0) ;
    fogCoords += screenOffset;
    float2 offsetCoords = frac(fogCoords + float2(time * -0.05, 0.0));
    float2 offsetCoords2 = frac(fogCoords + float2(time * 0.03, time * 0.01 + 0.3));
    
    float n = tex2D(spriteSampler, offsetCoords).r * 0.5;
    float n2 = tex2D(spriteSampler, offsetCoords2).r * 0.5;
    float combinedN = n + n2;
    float bottomNoise = 0.25;
    float inverse = 1.0 - bottomNoise;
    float diff = combinedN - bottomNoise;
    float alpha = diff / inverse;
    alpha *= 2.0;
    float dither = Dither(coords);
    return float4(alpha, alpha, alpha, alpha) * sampleColor - dither * 0.1 ;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}