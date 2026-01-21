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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    float3 ditheredColor = baseColor.rgb + ScreenSpaceDither(coords * uImageSize1, 5.0);
    baseColor.rgb = ditheredColor;
    baseColor.rgb = saturate(baseColor.rgb);
    return baseColor;

}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};