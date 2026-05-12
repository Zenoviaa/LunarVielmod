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
float grayscale(float3 rgb)
{
    
    float grey = max(rgb.r, rgb.g);
    grey = max(grey, rgb.b);
    return grey;
    
  //  return (rgb.r * 0.3 + rgb.g * 0.59 + rgb.b * 0.11);
}


float3 calculateColorGrayScale(float3 rgb)
{
    float currentGrayscale = grayscale(rgb);

    float3 closestColor = float3(0.0, 0.0, 0.0);
    float minDiff = 123;
    
    for (int x = 0; x < length; x++)
    {
        //Get the palette color
        float3 paletteColor = colors[x];
        
        //Check grayscale difference
        float grey = grayscale(paletteColor);
        float g = grey - currentGrayscale;
        float diff = abs(g);
        
        //No if statement, avoid branching
        float a = diff < minDiff;
        float b = 1.0 - a;
        minDiff = a * diff + b * minDiff;
        closestColor = a * paletteColor.rgb + b * closestColor;
    }
    
    return closestColor;
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
    
    float3 newColor = calculateColorGrayScale(processedColor);
    

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