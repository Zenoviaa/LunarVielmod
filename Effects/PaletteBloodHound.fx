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
    const float3 colors[16] =
    {
        float3(0.07450981, 0.011764706, 0.0627451),
        float3(0.11372549, 0.019607844, 0.09411765),
        float3(0.15294118, 0.02745098, 0.12941177),
        float3(0.23529412, 0.03529412, 0.12941177),
        float3(0.32156864, 0.043137256, 0.1254902),
        float3(0.49019608, 0.05882353, 0.12156863),
        float3(0.65882355, 0.078431375, 0.11372549),
        float3(0.827451, 0.09411765, 0.10980392),
        float3(1, 0.14509805, 0.16862746),
        float3(0, 0, 0),
        float3(0.0627451, 0.0627451, 0.0627451),
        float3(0.20392157, 0.20392157, 0.20392157),
        float3(0.32941177, 0.21568628, 0.21568628),
        float3(1, 0, 0),
        float3(1, 0.78431374, 0.78431374),
        float3(0, 0, 0)
    };


    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 16; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        if (currentDist < dist)
        {
            dist = currentDist;
            selectedColor = colors[i];
        }
    }
    float3 finalColor = lerp(color, selectedColor, uProgress);
    return finalColor;
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