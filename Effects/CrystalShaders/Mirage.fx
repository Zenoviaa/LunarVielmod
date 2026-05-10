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


float time;
float alpha;
float2 noiseSize;
texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 noiseCoords = coords * 0.2 + float2(time * -0.05, time * 0.05);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(noiseTex, noiseCoords).r;
    float4 color = tex2D(uImage0, coords) * sampleColor * lerp(1.0, 0.01, alpha);
    n *= lerp(0.0, 1.0, alpha) * 0.1;
    color.rgb += n * color.a;
    color.rgb = pow(color.rgb, lerp(1.0, 0.5, alpha));
    return color;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};