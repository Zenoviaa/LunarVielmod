sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler ditherSampler : register(s2);
float time;
float2 parallax;
float2 spriteSize;
float2 ditherTexelSize;
float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * spriteSize * ditherTexelSize;
    float dither = tex2D(ditherSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{  
    float3 gradientStartColor = float3(79.0 / 255.0, 103.0 / 255.0, 68.0 / 255.0);
    float3 gradientEndColor = float3(177.0 / 255.0, 188.0 / 255.0, 150.0 / 255.0);
    float3 gradientColor = lerp(gradientEndColor, gradientStartColor, coords.y);

    float2 distortingCoords = float2(time * 0.01, 0.05);
    float4 distortingNoiseColor = tex2D(noiseSampler, coords + distortingCoords);
    
    float2 noiseCoords = coords * 0.75;
    noiseCoords.x *= 0.55;
    noiseCoords.y += sin(time * 0.2 + distortingNoiseColor.r * 2.0) * 0.07;
    
    float noiseSpeed = 0.15;
    float2 scrollingCoords1 = noiseCoords + float2(time * -0.016 * noiseSpeed, time * -0.036 * noiseSpeed) + parallax * 0.05;
    float2 scrollingCoords2 = noiseCoords + float2(time * 0.016 * noiseSpeed, time * 0.025 * noiseSpeed + 0.3) + parallax * 0.025;
    float2 scrollingCoords3 = noiseCoords + float2(time * 0.011 * noiseSpeed, time * 0.02 * noiseSpeed + 0.3) + parallax * 0.012;

    float4 noiseColor = tex2D(noiseSampler, scrollingCoords1);
    float4 noiseColor2 = tex2D(noiseSampler, scrollingCoords2);
    float4 noiseColor3 = tex2D(noiseSampler, scrollingCoords3);
    
    float4 mixedColor = noiseColor + noiseColor2;
    mixedColor /= 2.0;
    mixedColor += -(1.0 * distortingNoiseColor - coords.y) * 0.5;
    mixedColor -= Dither(coords) * 0.05;
    
    mixedColor = floor(mixedColor * 8.0) / 8.0;
    mixedColor *= 1.4;
    mixedColor.rgb *= gradientColor;

    float4 baseColor = float4(gradientColor, 1.0);

    noiseColor3.rgb *= gradientEndColor;
    float4 finalColor = baseColor * 0.4 + mixedColor + noiseColor3 * 0.2;
    finalColor *= tintColor;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}