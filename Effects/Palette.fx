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
    const float3 colors[33] =
    {
        float3(0, 0, 0),
        float3(0.03529412, 0.03137255, 0.043137256),
        float3(0.078431375, 0.06666667, 0.09803922),
        float3(0.078431375, 0.06666667, 0.09803922),
        float3(0.11764706, 0.10980392, 0.14509805),
        float3(0.078431375, 0.06666667, 0.09803922),
        float3(0.11764706, 0.10980392, 0.14509805),
        float3(0.17254902, 0.1764706, 0.21960784),
        float3(0.2627451, 0.28235295, 0.31764707),
        float3(0.17254902, 0.1764706, 0.21960784),
        float3(0.2627451, 0.28235295, 0.31764707),
        float3(0.3764706, 0.41960785, 0.4392157),
        float3(0.44705883, 0.31764707, 0.21960784),
        float3(0.5921569, 0.41960785, 0.29411766),
        float3(0.18431373, 0.101960786, 0.29411766),
        float3(0.19215687, 0.15294118, 0.4862745),
        float3(0.23137255, 0.28235295, 0.65882355),
        float3(0.28235295, 0.5294118, 0.8039216),
        float3(0.3647059, 0.79607844, 0.9529412),
        float3(0.45882353, 0.9764706, 0.9764706),
        float3(1, 1, 1),
        float3(0.11764706, 0.10980392, 0.14509805),
        float3(0.16862746, 0.18431373, 0.15686275),
        float3(0.20392157, 0.24313726, 0.16470589),
        float3(0.3019608, 0.35686275, 0.29803923),
        float3(0.1882353, 0.4, 0.5137255),
        float3(0.14901961, 0.5529412, 0.43529412),
        float3(0.22745098, 0.78431374, 0.61960787),
        float3(0.43529412, 0.32156864, 0.45882353),
        float3(0.69411767, 0.5529412, 0.5686275),
        float3(0.89411765, 0.8156863, 0.6117647),
        float3(0.8745098, 0.99215686, 1),
        float3(0.8862745, 0.92941177, 0.99215686)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 33; i++)
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