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
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

//Vars
float2 screenOffset;
float2 tiling;
float3 innerColor;
float3 outerColor;
texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float2 tiledCoords = coords * tiling;
    tiledCoords += screenOffset;
    
    float noise1 = tex2D(noiseTex, tiledCoords).r;
    float noise2 = tex2D(noiseTex, tiledCoords + float2(0.05, 0.05)).r;
    float noise = noise1 + noise2;
    noise /= 2.0;
    noise = pow(noise, 0.5);
    
    float3 iceColor = lerp(outerColor, innerColor, noise);
    float levels = 8.0;
    iceColor = posterize(iceColor, levels);
    
    float4 finalColor = float4(iceColor, 1.0) * sampleColor;
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};