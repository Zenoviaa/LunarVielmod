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
texture2D windTexture;
sampler2D windTex = sampler_state
{
    texture = <windTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{   
    float2 uv = coords;
    uv.x *= 0.4;
    uv.y *= 4.0;
    float2 sampleCoords = uv + float2(time * -0.1, 0.0);
    sampleCoords.y += sin(time * 0.3 + uv.x * 9.0) * 0.3;
    sampleCoords = frac(sampleCoords);
    float4 flameCol = tex2D(windTex, sampleCoords);
    float4 finalColor = flameCol * sampleColor;
    finalColor.gb *= 0.25;
    finalColor *= sin(coords.x * 3.14);
    finalColor *= 1.2;
    finalColor = round(finalColor * 6.0) / 6.0;
    finalColor.a = 0;
    
    float4 screenColor = tex2D(uImage0, coords);
    screenColor += finalColor * 0.35;
    return screenColor;
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}