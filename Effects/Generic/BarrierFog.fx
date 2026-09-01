sampler maskTarget : register(s0);
sampler cloudSampler : register(s1);
sampler ditherSampler : register(s2);
float time;
float2 cloudTexelSize;
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

float4 CloudNoise(float2 screenUV, float2 offset)
{
    float2 fixedUV = screenUV * spriteSize * cloudTexelSize;
    fixedUV *= float2(0.3, 1.0);
    fixedUV += screenOffset;
    fixedUV += offset;
    fixedUV = frac(fixedUV);
    return tex2D(cloudSampler, fixedUV);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 mask = tex2D(maskTarget, coords);

    float n = CloudNoise(coords, float2(time * -0.05, 0.0)).r * 0.5;
    float n2 = CloudNoise(coords, float2(time * 0.03, time * 0.01 + 0.3)).r * 0.5;
    float combinedN = n + n2;
    float bottomNoise = 0.05;
    float inverse = 1.0 - bottomNoise;
    float diff = combinedN - bottomNoise;
    float alpha = diff / inverse;
    alpha *= 2.0;
    float dither = Dither(coords);
    

    return float4(alpha, alpha, alpha, alpha) * tintColor * mask - dither * 0.1;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}