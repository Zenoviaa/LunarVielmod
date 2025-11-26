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

float time;
float2 tiling;
float3 innerColor;
float3 outerColor;

texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

texture distortionTexture;
sampler2D distortionTex = sampler_state
{
    texture = <distortionTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

float distortion;
float2 DistortCoordinates(float2 coords)
{
    float n = tex2D(distortionTex, coords + float2(time * -0.1, 0.0));
    float2 distortedCoords = coords;
    distortedCoords.y += lerp(-1.0, 1.0, n) * distortion;
    distortedCoords.y = saturate(distortedCoords.y);
    return distortedCoords;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    float2 sampleCoords = coords * tiling + float2(time * -0.05, time * 0.05);
    float2 offsetCoords = DistortCoordinates(coords);
    sampleCoords += offsetCoords;
    
    float n = tex2D(noiseTex, sampleCoords);
    float3 color = lerp(innerColor, outerColor, n);
    float4 finalColor = float4(color, 1.0) * sampleColor;
    return baseColor * finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};