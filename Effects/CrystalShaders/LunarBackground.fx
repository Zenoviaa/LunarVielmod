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

float2 frontParallax;
float2 midParallax;
float2 farParallax;

float4 SampleParallaxing(sampler textureSampler, float2 coords, float2 parallax)
{
    float2 offsetCoords = coords + parallax;
    float2 normalCoords = float2(frac(offsetCoords.x), frac(offsetCoords.y));
    float4 backgroundColor = tex2D(textureSampler, normalCoords);
    return backgroundColor;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 farLayer = SampleParallaxing(uImage2, coords, farParallax);
    float4 midLayer = SampleParallaxing(uImage1, coords, midParallax);
    float4 closeLayer = SampleParallaxing(uImage0, coords, frontParallax);
    
    farLayer *= (1.0 - closeLayer.a) * (1.0 - midLayer.a);
    midLayer *= (1.0 - closeLayer.a);
    return (farLayer + midLayer + closeLayer) * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};