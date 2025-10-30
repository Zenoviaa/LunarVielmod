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
float size;

texture palette;
sampler2D paletteTex = sampler_state
{
    texture = <palette>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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
    
    for (float x = 0; x < size; x++)
    {
        //Get the palette color
        float3 paletteColor = tex2Dlod(paletteTex, float4((x + 0.5) / size, 0.5, 0, 0)).rgb;
        
        //Check grayscale difference
        float g = grayscale(paletteColor) - currentGrayscale;
        float diff = abs(g);
        
        //No if statement, avoid branching
        float a = diff < minDiff;
        float b = 1.0 - a;
        minDiff = a * diff + b * minDiff;
        closestColor = a * paletteColor + b * closestColor;
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