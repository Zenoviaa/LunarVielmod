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

texture petalTexture;
sampler2D petalTex = sampler_state
{
    texture = <petalTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

texture distortingNoiseTexture;
sampler2D distortingTex = sampler_state
{
    texture = <distortingNoiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float time;
float2 offset;
float2 tiling;
float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float bump = quadraticBump(coords.x);
    
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float4 screenColor = tex2D(uImage0, coords);
    
    float s = tex2D(distortingTex, coords + float2(time * -0.075, 0.0));
    float2 distortionOffsetCoords = float2(sin(s), cos(s)) * 0.1;
    
    float ySin = sin(coords.x * 8.0 + time * 0.05) * 0.0125;
    float2 petalOffsetCoords = float2(coords.x + time * -0.05, coords.y + ySin);
    float4 petal = tex2D(petalTex, petalOffsetCoords * tiling + distortionOffsetCoords + offset);
    petal *= bump;
    petal *= 0.035;
    
    float maxDistance = length(float2(0.0, 0.0) - float2(0.5, 0.5));
    float distance = length(coords - float2(0.5, 0.5));
    float interp = distance / maxDistance;
    float3 vignette = lerp(float3(0.0, 0.0, 0.0), float3(0.15, 0.15, 0.15), interp);
    
    screenColor.rgb += vignette;
    screenColor += petal;
    return screenColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};