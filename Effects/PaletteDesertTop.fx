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
    const float3 colors[35] =
    {
        float3(0.1254902, 0.06666667, 0.078431375),
        float3(0.1882353, 0.09019608, 0.101960786),
        float3(0.2901961, 0.039215688, 0),
        float3(0.3882353, 0.05882353, 0),
        float3(0.45490196, 0.14117648, 0.003921569),
        float3(0.56078434, 0.12941177, 0),
        float3(0.6313726, 0.2, 0),
        float3(0.7058824, 0.31764707, 0.007843138),
        float3(0.7764706, 0.4862745, 0.23529412),
        float3(0.78431374, 0.64705884, 0.5372549),
        float3(0.87058824, 0.8392157, 0.7647059),
        float3(0.9372549, 0.92941177, 0.9137255),
        float3(0.7137255, 0.7137255, 0.7058824),
        float3(0.52156866, 0.5921569, 0.49803922),
        float3(0.48235294, 0.49019608, 0.44705883),
        float3(0.41568628, 0.45882353, 0.43529412),
        float3(0.47058824, 0.4117647, 0.34901962),
        float3(0.3647059, 0.3372549, 0.3372549),
        float3(0.2784314, 0.3137255, 0.34901962),
        float3(0.31764707, 0.27450982, 0.3372549),
        float3(0.52156866, 0.21960784, 0.23137255),
        float3(0.32941177, 0.21176471, 0.20392157),
        float3(0.28627452, 0.16862746, 0.11372549),
        float3(0.12941177, 0.13333334, 0.14901961),
        float3(0, 0, 0),
        float3(0, 0, 0),
        float3(0, 0, 0),
        float3(0, 0, 0),
        float3(0.03137255, 0, 0.03137255),
        float3(0.19215687, 0.15686275, 0.21960784),
        float3(0.1764706, 0.1882353, 0.23921569),
        float3(0.15686275, 0.11764706, 0.2901961),
        float3(0.39215687, 0.3764706, 0.44313726),
        float3(0.7490196, 0.64705884, 0.627451),
        float3(0.92156863, 0.8784314, 0.8784314)
    };


    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 35; i++)
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