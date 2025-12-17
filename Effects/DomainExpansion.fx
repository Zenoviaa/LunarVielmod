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

float2 epicenter;
float radius;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{      
    // Normalized pixel coordinates (from 0 to 1)
#   define PI 3.14159
    float effectRadius = radius;
    float effectAngle = 2. * PI;
    float2 uv = coords - epicenter;
    
    float len = length(uv * float2(uScreenResolution.x / uScreenResolution.y, 1.));
    float angle = atan2(uv.y, uv.x) + effectAngle * smoothstep(effectRadius, 0., len);
    float radius = length(uv);

    float4 color = tex2D(uImage0, float2(radius * cos(angle), radius * sin(angle)) + epicenter);
    return color;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};