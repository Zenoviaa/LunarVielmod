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

texture primaryTexture;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 primaryTextureSize;
float2 resolution;
float2 screenOffset;
float2 parallax;
float gradientFade;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    //coords += uImageOffset;
 
    coords += parallax;
    //coords += uDirection;
 
    coords = frac(coords);
    float l = length(coords);
    float2 starNoiseCoords = (coords * resolution - uSourceRect.xy) / primaryTextureSize;
    float2 offset = uImageOffset;
    offset += coords * coords;
    
    float starNoise = tex2D(primaryTex, starNoiseCoords).r;
    
    float distortingNoise = tex2D(uImage2, (coords * sin(l * 50.0)) + float2(uTime * -0.03, uTime * -0.015)).r;
//    distortingNoise = 1.0;
    starNoise *= lerp(0, 1.4, distortingNoise);
    
    float4 finalColor = float4(starNoise, starNoise, starNoise, 0.0);
    finalColor *= uOpacity;
    
    float gf = lerp(1.0, 0.0, uv.y);
    float a = lerp(1.0, gf, gradientFade);
    finalColor *= a;
    return finalColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};