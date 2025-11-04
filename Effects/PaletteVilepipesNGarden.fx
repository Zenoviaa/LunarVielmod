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
    const float3 colors[22] =
    {
        float3(0.19215687, 0.20392157, 0.19607843),
        float3(0.19607843, 0.24313726, 0.25882354),
        float3(0.27058825, 0.29411766, 0.29411766),
        float3(0.22745098, 0.37254903, 0.23137255),
        float3(0.4862745, 0.27058825, 0.27058825),
        float3(0.40392157, 0.32156864, 0.22352941),
        float3(0.38431373, 0.3137255, 0.33333334),
        float3(0.31764707, 0.41960785, 0.2627451),
        float3(0.4745098, 0.42352942, 0.39215687),
        float3(0.44313726, 0.50980395, 0.27058825),
        float3(0.61960787, 0.5019608, 0.36078432),
        float3(0.6, 0.52156866, 0.4745098),
        float3(0.6745098, 0.5647059, 0.5254902),
        float3(0.6509804, 0.63529414, 0.5882353),
        float3(0.7058824, 0.67058825, 0.56078434),
        float3(0.7372549, 0.7176471, 0.64705884),
        float3(0.15294118, 0.1254902, 0.1254902),
        float3(0.08627451, 0.08627451, 0.14117648),
        float3(0.015686275, 0.09019608, 0.08235294),
        float3(1, 0, 0),
        float3(0.9764706, 1, 0),
        float3(0, 0, 0)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 22; i++)
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