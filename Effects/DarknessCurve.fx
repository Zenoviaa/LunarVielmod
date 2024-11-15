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
float whiteCurve;
float blackCurve;

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
    const float3 colors[2] =
    {
        float3(0, 0, 0),
        float3(1, 1, 1)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;
    int selIndex = 0;
    // For loop with the same loops than the color palette.
    for (int i = 1; i < 2; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        if (currentDist < dist)
        {
            selIndex = i;
            dist = currentDist;
            selectedColor = colors[i];
        }
    }
    
    if (selIndex == 0)
    {
        float3 finalColor = lerp(color, selectedColor, uProgress * blackCurve);
        return finalColor;
    }
    else
    {
        float3 finalColor = lerp(color, selectedColor, uProgress * whiteCurve);
        return finalColor;
    }
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