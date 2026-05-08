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
float frequency;
float amplitude;
float xStrength;
texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};
float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 offsetCoords = coords;
    float xOffset = coords.x * xStrength;
    offsetCoords += float2(0.0, sin(time * frequency + xOffset) * amplitude);
  
    
  //  offsetCoords.x += time;
   // offsetCoords.x = frac(offsetCoords.x);
 //   offsetCoords.y *= coords.x;
    float4 color = tex2D(uImage0, offsetCoords);
    
    float2 noiseCoords = coords + float2(0.0, time * 1.5);
    noiseCoords = frac(noiseCoords);
    noiseCoords.y *= coords.x;
    float n = tex2D(noiseTex, noiseCoords).r;
    return (color * sampleColor * 3.0 + color.r * n * 2.0) * QuadraticBump(coords.x) * lerp(0.0, 1.0, coords.y) * lerp(0.0, 1.0, coords.x);

}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};