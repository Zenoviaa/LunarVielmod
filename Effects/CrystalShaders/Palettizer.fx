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
bool dither;
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

//http://loopit.dk/banding_in_games.pdf
//reference: https://www.shadertoy.com/view/4dcSRX
float3 ScreenSpaceDither(float2 vScreenPos, float colorDepth)
{
    // lestyn's RGB dither (7 asm instructions) from Portal 2 X360, slightly modified for VR
    float d = dot(float2(131.0, 312.0), vScreenPos.xy);
    float3 vDither = float3(d, d, d);
    vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
    return (vDither.rgb / colorDepth) * 0.375;
}

float3 DitherV2(float2 uv)
{
    float2x2 bayerMatrix = float2x2(
        -0.375, 0.125,
        0.375, -0.125);
    float n = 2.0;
    float2 modUV = float2(fmod(uv.x * 1920.0, n), fmod(uv.y * 1080.0 * -1.0, n));
    float ditherStrength = spread * (mul(bayerMatrix, modUV));
    return float3(ditherStrength, ditherStrength, ditherStrength) * ditherAlpha;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
       //Dither as close as possible to the color quantization
    if (dither)
    {
        float3 ditheredColor = baseColor.rgb + ScreenSpaceDither(coords * uImageSize1, 1.0) * ditherAlpha;
        baseColor.rgb = ditheredColor;
        baseColor.rgb = saturate(baseColor.rgb);
    }
        //The colors bug out if it ever reaches 1, so we need to just make it barely under
    //Smh this is stupid, so the bug was with the texture sampling.
    baseColor.rgb *= 0.99;
  
    float4 colorToMapTo = tex3D(ColorSpectrumTextureSampler, baseColor.rgb);
    baseColor.rgb = lerp(baseColor.rgb, colorToMapTo.rgb, uProgress);
    return baseColor;
}


technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};