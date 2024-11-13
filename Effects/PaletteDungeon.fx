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
    const float3 colors[14] =
    {
        float3(0, 0, 0),
        float3(0, 0, 0),
        float3(0.015686275, 0.015686275, 0.015686275),
        float3(0.05882353, 0.05882353, 0.05882353),
        float3(0.13333334, 0.18039216, 0.17254902),
        float3(0.17254902, 0.15294118, 0.22352941),
        float3(0.15686275, 0.2509804, 0.21176471),
        float3(0.19215687, 0.17254902, 0.30588236),
        float3(0.23529412, 0.2, 0.34117648),
        float3(0.28627452, 0.21960784, 0.40784314),
        float3(0.21176471, 0.35686275, 0.2784314),
        float3(0.37254903, 0.25490198, 0.4745098),
        float3(0.30588236, 0.5176471, 0.39215687),
        float3(0.29411766, 0.54509807, 0.4509804)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 14; i++)
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