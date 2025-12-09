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
float mipBias;


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

float4 getTexure(float2 coords)
{
    return tex2D(uImage0, coords);
}

float4 calculateBlur(float2 coords)
{
    const int samples = 8;
    float4 color;
    float2 texelSize = mipBias / uScreenResolution.xy;
    for (int i = 0; i < samples; i++)
    {
        
        float2 coord0 = coords + (float2(i, i) * texelSize / float(samples));
        float2 coord1 = coords + (float2(-i, -i) * texelSize / float(samples));
        float2 coord2 = coords + (float2(-i, i) * texelSize / float(samples));
        float2 coord3 = coords + (float2(i, -i) * texelSize / float(samples));
            
        color += getTexure(coord0);
        color += getTexure(coord1);
        color += getTexure(coord2);
        color += getTexure(coord3);
    }
    
    color /= float(samples);
    return (color / 4.0);
}



float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 blur = calculateBlur(input.TextureCoordinates.xy);
    return blur * input.Color;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};