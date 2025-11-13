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

float InExpo(float t)
{
    const float p = 10;
    return t == 0 ? 0 : pow(2, p * t - p);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    //Let's try

    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float4 color = float4(1.0, 1.0, 1.0, 1.0);
    color *= 1.0 - pow(dist, 5.0);
    return color;
}


technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};