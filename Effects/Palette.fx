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
float3 Palette[64];
int PaletteLength;

        

float colorDistance(float3 a, float3 b)
{
    float d = sqrt(pow((b.r - a.r), 2) + pow((b.g - a.g), 2) + pow((b.b - a.b), 2));
    return d;
}

float colorDistance2(float3 a, float3 b)
{
    float ar = abs(b.r - a.r);
    float ag = abs(b.g - a.g);
    float ab = abs(b.b - a.b);
    float d = ar + ag + ab;
    return d;
}

float3 calculateColor(float3 color)
{
	// Palette 1
    const float3 colors[8] =
    {
        float3(0.137, 0.118, 0.012),
        float3(0.278, 0.251, 0.039),
        float3(0.427, 0.404, 0.118),
        float3(0.576, 0.561, 0.224),
        float3(0.71, 0.702, 0.353),
        float3(0.804, 0.808, 0.533),
        float3(0.89, 0.898, 0.718),
        float3(0.984, 0.988, 0.949)
    };
    
    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    
    // For loop with the same loops than the color palette.
    for (int i = 1; i < 8; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        if (currentDist < dist)
        {
            dist = currentDist;
            selectedColor = colors[i];
        }
    }
    
    return selectedColor;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 newColor = calculateColor(color.rgb);
    color.rgb = newColor;
    return color;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};