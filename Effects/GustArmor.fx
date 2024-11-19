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
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
matrix uWorldViewProjection;
float4 uShaderSpecificData;
float uStretchReverseFactor;

float time;
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
float2 noiseTextureSize;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

// The X coordinate is the trail completion, the Y coordinate is the elevation on the point of the trail.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Calculate distorting noise
    float2 coords = input.TextureCoordinates;

    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy) / noiseTextureSize;
    float sampleNoiseColor = tex2D(noiseTex, noiseCoords + float2(time * 0.05, time * 0.025));
    float4 sampleColor = tex2D(uImage0, coords);
  
    if (sampleColor.b == 1)
    {
        sampleColor *= 0;
        return sampleColor;
    }
    else
    {
        float alpha = lerp(0.0, 1.0, pow(sampleNoiseColor, 4.0));
        return sampleColor * alpha;
    }
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
