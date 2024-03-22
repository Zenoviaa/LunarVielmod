sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uSaturation;
float uOpacity;
float uTime;
float uRotation;
float uDirection;
float4 uSourceRect;
float4 uLegacyArmorSourceRect;
float2 uTargetPosition;
float2 uWorldPosition;
float2 uImageSize0;
float2 uLegacyArmorSheetSize;
float2 uImageSize1;
float3 uLightSource;
float4 uShaderSpecificData;

//Sprite Initially made by DomonicKarma, edited by Lion8cake to fit the depth's needs
// The usage of these two methods seemingly prevents imprecision problems for some reason.
float2 convertToScreenCoords(float2 coords)
{
    return coords * uImageSize0;
}

float2 convertFromScreenCoords(float2 coords)
{
    return coords / uImageSize0;
}

float4 PixelOffsetColor(float2 coords, float2 offset)
{
    float2 offsetCoords = saturate(convertFromScreenCoords(convertToScreenCoords(coords) + offset));
    return tex2D(uImage0, offsetCoords);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the 0-1 coords value relative to whatever frame in the texture is being used.
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    
    // Get the pixel's color on the base texture, and take the sample color into account.
    float4 color = tex2D(uImage0, coords) * sampleColor;
    
    // Calculate the color value of neighboring pixels.
    float4 left = PixelOffsetColor(coords, float2(-2, 0));
    float4 right = PixelOffsetColor(coords, float2(2, 0));
    float4 top = PixelOffsetColor(coords, float2(0, -2));
    float4 bottom = PixelOffsetColor(coords, float2(0, 2));
    
    // Make pixel black by default.
    color *= float4(0, 0, 0, 1);
    
    // Make colors that neighbor an invisible pixel red.
    float redInterpolant = saturate((left.a <= 0) + (right.a <= 0) + (top.a <= 0) + (bottom.a <= 0));
    
    // Combine colors.
    return lerp(color, float4(float3(255, 255, 255) / 255, 1) * color.a * sampleColor.a, redInterpolant);
}

technique Technique1
{
    pass SilhouettePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
   