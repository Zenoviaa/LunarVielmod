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

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 direction = (coords - float2(0.5, 0.5));
    float angle = atan2(direction.y, direction.x);
    float normalAngle = angle / 3.14 * 0.5f + 0.5f;
    
    float dist = length(direction);
    float progress = saturate(dist / 0.5);
    progress = 1.0f - progress;
    float strength = progress * sampleColor.a;
    
    //Store the angle i nthe red channel and how far it pushes in the strength channel
    float4 pushColor = float4(normalAngle, strength * 0.08, 0.0, 1.0);
    return pushColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}