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

float3 getTexure(float2 coords)
{
    return tex2D(uImage0, coords).rgb;
}

float4 getCoords(float2 coords, float mipBias)
{
    float4 newCoords = float4(coords.x, coords.y, 0.0, mipBias);
    return newCoords;
}
float3 calculateBlur(float2 coords, float mipBias)
{

    float2 texelSize = mipBias / uScreenResolution.xy;
    
    float4 Color = tex2Dbias(uImage0, getCoords(coords, mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(texelSize.x, 0.0), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(-texelSize.x, 0.0), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(0.0, texelSize.y), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(0.0, -texelSize.y), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(texelSize.x, texelSize.y), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(-texelSize.x, texelSize.y), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(texelSize.x, -texelSize.y), mipBias));
    Color += tex2Dbias(uImage0, getCoords(coords + float2(-texelSize.x, -texelSize.y), mipBias));

    return Color / 9.0;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 blur = calculateBlur(coords, uProgress);

    float blurSize = bloom.x;
    float intensity = bloom.y;
    float threshold = bloom.z;
    float3 result = clamp(calculateBlur(coords, blurSize) - threshold, 0.0, 1.0) * 1.0 / (1.0 - threshold);
    float4 highlight = float4(result, 1.0);
        
    float4 finalColor = 1.0 - (1.0 - color) * (1.0 - highlight * intensity);
    return finalColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};