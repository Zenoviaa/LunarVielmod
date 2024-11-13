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
    const float3 colors[28] =
    {
        float3(0, 0, 0),
        float3(0.09411765, 0.05490196, 0.11372549),
        float3(0.101960786, 0.101960786, 0.101960786),
        float3(0.11372549, 0.06666667, 0.13725491),
        float3(0.15686275, 0.09019608, 0.18039216),
        float3(0.1254902, 0.101960786, 0.20392157),
        float3(0.16862746, 0.16862746, 0.16862746),
        float3(0.21960784, 0.15686275, 0.31764707),
        float3(0.27058825, 0.13725491, 0.25490198),
        float3(0.38431373, 0.19215687, 0.3647059),
        float3(0.30588236, 0.19215687, 0.38431373),
        float3(0.3764706, 0.21568628, 0.4627451),
        float3(0.4392157, 0.21568628, 0.4627451),
        float3(0.2901961, 0.2901961, 0.2901961),
        float3(0.45882353, 0.45882353, 0.45882353),
        float3(0.27450982, 0.25882354, 0.5529412),
        float3(0.34509805, 0.32941177, 0.6862745),
        float3(0.30588236, 0.41960785, 0.84705883),
        float3(0.8156863, 0.23529412, 0.70980394),
        float3(0.5568628, 0.25882354, 0.5568628),
        float3(0.6431373, 0.27058825, 0.5411765),
        float3(0.8039216, 0.34901962, 0.6862745),
        float3(0.9254902, 0.4627451, 0.7647059),
        float3(0.43137255, 0.5882353, 0.9607843),
        float3(0.6627451, 0.6627451, 0.6627451),
        float3(0.9490196, 0.6431373, 0.7882353),
        float3(0.84313726, 0.8980392, 0.9882353),
        float3(0.93333334, 0.93333334, 0.93333334)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 28; i++)
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