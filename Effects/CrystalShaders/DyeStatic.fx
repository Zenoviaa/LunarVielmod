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
float time;
bool dirty;

float4 noise(float2 texCoord)
{
    const float e = 2.7182818284590452353602874713527;
    float G = e + (time * 0.1);
    float2 r = (G * sin(G * texCoord.xy));
    float f = frac(r.x * r.y * (1.0 + texCoord.x));
    return float4(f, f, f, f);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Calculate Distortion
    float4 sampleColor = tex2D(uImage0, coords);
    if (sampleColor.a > 0)
    {
        if (dirty)
        {
            float4 noiseSample = noise(coords);
            sampleColor.rgb *= noiseSample.rgb;
            return sampleColor;
        }
        else
        {
            float4 noiseSample = noise(coords);
            sampleColor.rgb = noiseSample.rgb;
            return sampleColor;
        }
    }
    else
    {
        return sampleColor;
    }
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}