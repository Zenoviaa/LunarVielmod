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
    const float3 colors[25] =
    {
        float3(0.22352941, 0.17254902, 0.19215687),
        float3(0.2901961, 0.23529412, 0.2901961),
        float3(0.3529412, 0.33333334, 0.3529412),
        float3(0.38431373, 0.4117647, 0.41568628),
        float3(0.4509804, 0.5058824, 0.48235294),
        float3(0.5137255, 0.5372549, 0.54509807),
        float3(0.5137255, 0.5686275, 0.54509807),
        float3(0.6431373, 0.6156863, 0.6431373),
        float3(0.77254903, 0.69803923, 0.7411765),
        float3(0.8352941, 0.74509805, 0.8039216),
        float3(0.87058824, 0.8392157, 0.87058824),
        float3(0.9019608, 0.91764706, 0.93333334),
        float3(0, 0, 0.050980393),
        float3(0.05490196, 0.06666667, 0.14901961),
        float3(0.21960784, 0.19607843, 0.16078432),
        float3(0.09411765, 0.11764706, 0.31764707),
        float3(0.09803922, 0.21176471, 0.4627451),
        float3(0.25490198, 0.2784314, 0.31764707),
        float3(0.39607844, 0.30980393, 0.4627451),
        float3(0.6509804, 0.41568628, 0.007843138),
        float3(1, 1, 0.24313726),
        float3(0.9372549, 0.6392157, 0.3137255),
        float3(0.15686275, 0.3137255, 0.5803922),
        float3(0.37254903, 0.37254903, 0.68235296),
        float3(0.20392157, 0.5176471, 0.68235296)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 25; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        //Branchless way to do this
        //We want to avoid using if-statements in shaders if possible, as creating branches GREATLY slows them down
        //We can evaluate a check like this to a 0 or 1, and since only 1 can be true we can invert it simply :) 
        float a = currentDist < dist;
        float b = 1.0 - a;
        dist = a * currentDist + b * dist;
        selectedColor = a * colors[i] + b * selectedColor;
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