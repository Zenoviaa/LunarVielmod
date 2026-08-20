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
    //Distorting Big Rek Balls
    float2 distortionNoiseCoords = coords + float2(time * -0.03, 0.3);
    distortionNoiseCoords = frac(distortionNoiseCoords);
    distortionNoiseCoords *= 0.2;
    float distortingNoise = tex2D(noiseSampler, distortionNoiseCoords).r;
    float2 offCoords = coords;
    offCoords.y += sin(distortingNoise * 3.14) * 0.2 - 0.2;
   
    float2 noiseCoords = offCoords + float2(time * -0.05, 0.0);
    noiseCoords = frac(noiseCoords);
    float tiling = lerp(0.3, 1.0, coords.x);
    
    //Hope this works?
    noiseCoords *= tiling;
  
    noiseCoords.y += sin(distortingNoise ) * strength;
    float noise = tex2D(noiseSampler, noiseCoords).r;
    noise *= 1.4;
    float3 particleColor = lerp(innerColor, bloomColor, noise);
    
    float2 diff = offCoords - float2(0.5, 0.5);
    float fade = saturate(length(diff) / 0.35);
    fade = 1.0 - fade;

    particleColor.rgb += smoothstep(0.0, 1.0, offCoords.x);
    particleColor.b -= 0.3;
    

    float4 finalcolor = float4(particleColor, 1.0) * sampleColor * fade * 8.0;
    return finalcolor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}