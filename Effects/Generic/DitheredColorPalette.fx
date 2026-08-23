sampler screenSpriteSampler : register(s0);
sampler ditherSpriteSampler : register(s1);
float2 screenSize;
float2 ditherTexelSize;
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


float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * screenSize * ditherTexelSize;
    float dither = tex2D(ditherSpriteSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 baseColor = tex2D(screenSpriteSampler, coords);
    float r = baseColor.r;
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
    baseColor.rgb = colorToMapTo.rgb * r;
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