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
float3 bloom;
float mipBias;
float2 sunDirection;
float falloff;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};


float4 getCoords(float2 coords, float mipBias)
{
    float4 newCoords = float4(coords.x, coords.y, 0.0, mipBias);
    return newCoords;
}

float4 calculateBlur(VertexShaderOutput input, float mipBias)
{
    float2 texelSize = mipBias / uScreenResolution.xy;
    
    //Basically what we're going to do is offset the coords and see if it lands in a shadow
    //If it does then we blend that color based on the distance
    const float samples = 8.0;
    float4 color = float4(0.0, 0.0, 0.0, 0.0);
    for (float i = 0; i < samples; i++)
    {
        float2 offset = sunDirection * texelSize * (i / samples);
        float2 coords = input.TextureCoordinates.xy;
        float2 offsetCoords = coords + offset;
        float4 biasedCoords = getCoords(offsetCoords, mipBias);
        float4 shadowColor = tex2Dbias(uImage0, biasedCoords);
        
        
        //Calculate fall off so we get a smoothing effect
        float falloffFactor = distance(coords, offsetCoords) / falloff;
        falloffFactor = saturate(falloffFactor);
        falloffFactor = 1.0 - falloffFactor;
        color += shadowColor * falloffFactor;
    }
    
    //average out the result
    color /= samples;
    return color;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return calculateBlur(input, mipBias) * input.Color;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};