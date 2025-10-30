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

    float3 selectedColor = tex2D(uImage1, float2(0.0, 0.0));
    float dist = colorDistance2(color, selectedColor);
    float currentDist;

    // For loop with the same loops than the color palette.
    const int maxColors = 32;
    for (int i = 1; i < maxColors; i++)
    {
        //Non array version to do this, pretty sure this would be slower?
        //There's more instructions than the array so there's no shot it's faster
      
        float2 coords = float2(i / maxColors, 0.0);
        float3 sampleColor = tex2D(uImage1, coords);
        currentDist = colorDistance2(color, sampleColor);
        
        //Branchless way to do this
        //We want to avoid using if-statements in shaders if possible, as creating branches GREATLY slows them down
        //We can evaluate a check like this to a 0 or 1, and since only 1 can be true we can invert it simply :) 
        float a = currentDist < dist;
        float b = 1.0 - a;
        dist = a * currentDist + b * dist;
        selectedColor = a * sampleColor + b * selectedColor;
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