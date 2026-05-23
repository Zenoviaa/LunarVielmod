
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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float2 noiseCoords = uv + float2(time * -0.05, time * -0.05);
    float2 noiseCoords2 = uv + float2(time * 0.05, time * -0.05 + 0.4);

    noiseCoords = frac(noiseCoords);
    noiseCoords2 = frac(noiseCoords2);
    float n = tex2D(uImage0, noiseCoords);
    float n2 = tex2D(uImage0, noiseCoords2);
    float combinedNoise = saturate((n + n2) / 2.0);
    return float4(combinedNoise, combinedNoise, combinedNoise, combinedNoise) * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};