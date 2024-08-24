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

float uColorMapSection;



float4 ColorMap(float strength)
{
    return tex2D(uImage2, float2(clamp(strength, 0.01, 0.99), uColorMapSection));
}

/*

float noise(float2 coords)
{
    return tex2D(uImage1, coords);
}

float fbm(float2 coords)
{
    float value = 0.0f;
    float scale = 0.5f;

    for (int i = 0; i < 3; i++)
    {
        value += noise(coords) * scale;
        coords *= 4.0f;
        scale *= 0.5f;
    }

    return value;
}

float value(float2 uv)
{
    float Pixels = 1024.0;
    Pixels *= 500.0;
    float dx = 10.0 * (1.0 / Pixels);
    float dy = 10.0 * (1.0 / Pixels);
  
    float final = 0.0f;  
    float2 uvc = uv;  
    float2 Coord = float2(dx * floor(uvc.x / dx),
                          dy * floor(uvc.y / dy));
 
    for (int i = 0; i < 3; i++)
    {
        float f = fbm(Coord + uTime * uIntensity * -0.01f + float2(i, i));
        float2 motion = float2(f, f);
        motion += uImageOffset;
        final += fbm(Coord + motion + float2(i, i));
    }

    return final / 3.0f;
}
*/
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float time = uTime;
    float noise1 = tex2D(uImage1, coords * 4 + float2(time * -0.03, 0.0) + uImageOffset);
    float noise2 = tex2D(uImage1, coords * 3 + float2(time * -0.007 - 0.51, 0.0) + uImageOffset);
    float noise3 = tex2D(uImage1, coords * 2 + float2(time * -0.0039 + 0.83, 0.0) + uImageOffset);
    float noise = (noise1 + noise2 + noise3) * 0.4;
    
    float4 color = tex2D(uImage1, coords) * noise * 0.4;
    return lerp(float4(0.0, 0.0, 0.0, 0.0), float4(color.r, color.g, color.b, 0.0), uIntensity) * uIntensity;
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}