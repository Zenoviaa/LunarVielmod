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
float spread;
float ditherAlpha;
Texture3D ColorSpectrumTexture;
sampler3D ColorSpectrumTextureSampler = sampler_state
{
    Texture = <ColorSpectrumTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

float2 ditherTexelSize;
float2 screenOffset;


//http://loopit.dk/banding_in_games.pdf
//reference: https://www.shadertoy.com/view/4dcSRX


float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * uImageSize1 * ditherTexelSize;
    ditherTextureUV += screenOffset;
    float dither = tex2D(uImage1, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
       
    //Dither as close as possible to the color quantization
    float texBrightness = max(baseColor.r, max(baseColor.g, baseColor.b));
    float ditherColor = Dither(coords);
  
    float3 ditheredColor = baseColor.rgb - ditherColor * ditherAlpha;
    baseColor.rgb = ditheredColor;
    baseColor.rgb = saturate(baseColor.rgb);
    
    //The colors bug out if it ever reaches 1, so we need to just make it barely under
    //Smh this is stupid, so the bug was with the texture sampling.
    baseColor.rgb *= 0.99;
  
    float4 colorToMapTo = tex3D(ColorSpectrumTextureSampler, baseColor.rgb);
    baseColor.rgb = lerp(baseColor.rgb, colorToMapTo.rgb, uProgress);
    baseColor *= tintColor;
    return baseColor;
}


technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};