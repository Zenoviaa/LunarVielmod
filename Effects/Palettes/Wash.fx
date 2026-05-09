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

float3 colors[16];
float length;
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
    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < length; i++)
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


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 color = tex2D(uImage0, coords) ;
    if(color.a == 0.0)
        return float4(0.0, 0.0, 0.0, 0.0);
    float3 processedColor = color.rgb;
    float Brightness = 0;
    float Contrast = 1;
    float GammaCorrection = 1.15f;
    
    float3 gammaFactor = float3(GammaCorrection, GammaCorrection, GammaCorrection);
    processedColor = pow(processedColor, gammaFactor);
    processedColor.r = clamp(processedColor.r, 0.0, 1.0);
    processedColor.g = clamp(processedColor.g, 0.0, 1.0);
    processedColor.b = clamp(processedColor.b, 0.0, 1.0);
    
    float3 newColor = calculateColor(processedColor);
    

    color.rgb = newColor;
                   
                
    return color * sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}