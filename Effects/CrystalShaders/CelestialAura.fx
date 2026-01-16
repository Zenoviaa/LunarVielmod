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

//Vars
float time;
float3 innerColor;
float3 outerColor;
float2 tiling;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float2 polarUV = float2(angle, dist);
    return polarUV;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float2 polarUV = PolarCoordinates(coords);
    polarUV += float2(time, time);
    polarUV *= tiling;
    
    //Take the frac so it wraps properly
    polarUV.x = frac(polarUV.x);
    polarUV.y = frac(polarUV.y);
    float r = tex2D(uImage0, polarUV);
    
    //Apply vignette bloom
    float maxDistance = length(float2(0.0, 0.0) - float2(0.5, 0.5));
    float distance = length(coords - float2(0.5, 0.5));
    float interp = distance / maxDistance;
    float3 color = lerp(innerColor, outerColor, interp);
    float4 myColor = float4(color * r * sampleColor.rgb, sampleColor.a);
    return myColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};