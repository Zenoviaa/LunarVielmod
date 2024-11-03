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
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
matrix uWorldViewProjection;
float4 uShaderSpecificData;
float uStretchReverseFactor;

float time;
float distortionStrength;
float2 movementVelocity;
float startPixel;
float endPixel;
texture windNoiseTexture;
sampler2D windNoiseTex = sampler_state
{
    texture = <windNoiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

// The X coordinate is the trail completion, the Y coordinate is the elevation on the point of the trail.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Calculate distorting noise
    float2 coords = input.TextureCoordinates;
    float s = tex2D(windNoiseTex, coords + float2(time * -0.5, time * -0.2));
    float2 oCoords = coords;
    
    float pixel = coords.y * uImageSize0.y;
    float progress = 0.0;
    if (pixel >= startPixel && pixel <= endPixel)
    {
        progress = (pixel - startPixel) / (endPixel - startPixel);
    }

    oCoords.x += s * distortionStrength * progress;
    oCoords.y += s * distortionStrength;
    
    //Offset according to this
    oCoords.x += -movementVelocity.x * progress;
    oCoords.y += -movementVelocity.y * progress;
    
    //Color
    float4 color = tex2D(uImage0, oCoords) * input.Color;
    return color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
