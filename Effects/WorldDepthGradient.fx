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
float3 gradientStrength;
float3 gradientColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
 //   return float4(1.0, 1.0, 0.5, 1.0);
    float3 gradingColor = lerp(gradientColor * gradientStrength.x, gradientColor * gradientStrength.y, coords.y);
    float4 color = tex2D(uImage0, coords);
    color.rgb += gradingColor * gradientStrength.z; //= lerp(color.rgb, gradingColor, gradientStrength.z);
    return color;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};