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


float grayscale(float3 rgb)
{
    return (rgb.r * 0.3 + rgb.g * 0.59 + rgb.b * 0.11);
}



float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 currentColor = tex2D(uImage0, coords);
    float currentGrayscale = grayscale(currentColor.rgb);

    float3 closestColor = float3(0.0, 0.0, 0.0);
    float minDiff = 123;
    
    const float4 colors[22] =
    {
        float4(0, 0, 0, 0),
        float4(0.078431375, 0.06666667, 0.09803922, 0.07364706),
        float4(0.11764706, 0.10980392, 0.14509805, 0.11603922),
        float4(0.17254902, 0.1764706, 0.21960784, 0.18003923),
        float4(0.2627451, 0.28235295, 0.31764707, 0.28035295),
        float4(0.44705883, 0.31764707, 0.21960784, 0.3456863),
        float4(0.5921569, 0.41960785, 0.29411766, 0.45756865),
        float4(0.18431373, 0.101960786, 0.29411766, 0.14780392),
        float4(0.19215687, 0.15294118, 0.4862745, 0.20137255),
        float4(0.23137255, 0.28235295, 0.65882355, 0.3084706),
        float4(0.28235295, 0.5294118, 0.8039216, 0.4854902),
        float4(0.3647059, 0.79607844, 0.9529412, 0.6839216),
        float4(0.45882353, 0.9764706, 0.9764706, 0.8211764),
        float4(1, 1, 1, 1),
        float4(0.16862746, 0.18431373, 0.15686275, 0.17658824),
        float4(0.3019608, 0.35686275, 0.29803923, 0.33392158),
        float4(0.14901961, 0.5529412, 0.43529412, 0.41882354),
        float4(0.22745098, 0.78431374, 0.61960787, 0.5991372),
        float4(0.43529412, 0.32156864, 0.45882353, 0.37078434),
        float4(0.69411767, 0.5529412, 0.5686275, 0.5970197),
        float4(0.89411765, 0.8156863, 0.6117647, 0.81678426),
        float4(0.8745098, 0.99215686, 1, 0.95772547)
    };

    for (int x = 0; x < 22; x++)
    {
        //Get the palette color
        float4 paletteColor = colors[x];
        
        //Check grayscale difference
        float g = paletteColor.a - currentGrayscale;
        float diff = abs(g);
        
        //No if statement, avoid branching
        float a = diff < minDiff;
        float b = 1.0 - a;
        minDiff = a * diff + b * minDiff;
        closestColor = a * paletteColor.rgb + b * closestColor;
    }
    
    return float4(closestColor, currentColor.a);
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};