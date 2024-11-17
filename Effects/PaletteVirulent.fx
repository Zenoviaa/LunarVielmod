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
        float3(0.49019608, 0.21960784, 0.2509804),
        float3(0.050980393, 0.03137255, 0.050980393),
        float3(0.16470589, 0.13725491, 0.28627452),
        float3(0.25490198, 0.5019608, 0.627451),
        float3(0.19607843, 0.3254902, 0.37254903),
        float3(0.45490196, 0.6784314, 0.73333335),
        float3(0.48235294, 0.69803923, 0.30588236),
        float3(1, 0.9764706, 0.89411765),
        float3(0.74509805, 0.73333335, 0.69803923),
        float3(0.9843137, 0.8745098, 0.60784316),
        float3(0.9411765, 0.7411765, 0.46666667),
        float3(0.77254903, 0.5686275, 0.32941177),
        float3(0.50980395, 0.35686275, 0.19215687),
        float3(0.9098039, 0.6, 0.4509804),
        float3(0.75686276, 0.42352942, 0.35686275),
        float3(0.30980393, 0.16862746, 0.14117648)
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