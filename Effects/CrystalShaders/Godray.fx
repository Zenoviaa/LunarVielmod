sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float2 uTargetPosition;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    l = pow(l, 1.5);
    if (l > 0.5)
    {
        l = 0.5;
    }
 
    float p = l / 0.5;
    p = pow(p, 2.5);
    if (p > 1.0)
    {
        p = 1.0;
    }
 
   
    color.rgb += (color.rgb / 2.0) * sin(uTime);
    color.rgb *= uColor;
    color.a = 0.0;
    color *= (1.0 - p);
    color *= sampleColor;
    return color;
}

technique Technique1
{
    pass GlowingDustPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}