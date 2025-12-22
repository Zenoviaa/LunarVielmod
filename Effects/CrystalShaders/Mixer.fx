sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float time;
float strength;

texture mixTexture;
sampler2D mixTextureSampler = sampler_state
{
    texture = <mixTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture noiseTexture;
sampler2D noiseTextureSampler = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float3 SampleStars(float2 uv)
{
    float3 col = tex2D(mixTextureSampler, uv).rgb;
    return col;
}

float SampleDistortion(float2 uv)
{
    float noise = tex2D(noiseTextureSampler, uv * 0.1).r;
    return noise;
}

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}

float3 posterize(in float3 color, float factor)
{
    color.r = posterize(color.r, factor);
    color.g = posterize(color.g, factor);
    color.b = posterize(color.b, factor);
    return color;
}


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    //Calculate Distortion
    float noise = SampleDistortion(uv + float2(time * -0.05, time * 0.05));
    float2 angleOffset = float2(sin(noise), cos(noise));
    uv += angleOffset * strength;
    
    // Time varying pixel color
    float2 starsUv1 = uv + float2(time * -0.05, 0.1);
    float2 starsUv2 = uv + float2(time * 0.05, 0.0);
    float3 stars1 = SampleStars(starsUv1);
    float3 stars2 = SampleStars(starsUv2);
    float3 stars = stars1 + stars2;
    stars *= 0.5;
      
    float posterizationLevels = 8.0;
    stars = posterize(stars, posterizationLevels);
    float4 color = float4(stars, 1.0);
    color *= sampleColor;
    return color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}