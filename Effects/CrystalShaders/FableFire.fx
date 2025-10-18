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
float3 innerColor;
float3 outerColor;
float3 glowColor;
float time;
float distortion;
float2 tiling;
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
texture noiseTexture2;
sampler2D noiseTex2 = sampler_state
{
    texture = <noiseTexture2>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture noiseTexture3;
sampler2D noiseTex3 = sampler_state
{
    texture = <noiseTexture3>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 DistortCoordinates(float2 coords)
{
    float noise = tex2D(noiseTex, (coords + float2(0.0, time * -0.08)) * tiling);
    float2 distortedCoordinates = coords;
    distortedCoordinates.x += smoothstep(-1.0, 1.0, noise) * distortion;
    distortedCoordinates.x = saturate(distortedCoordinates.x);
    return distortedCoordinates;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //So we have the mask
    //First we're gonna distort the mask texture
    float2 distortedCoords = DistortCoordinates(coords);
    float mask = tex2D(uImage0, distortedCoords);
    
    //Sample noise
    float n1 = tex2D(noiseTex, (coords + float2(0.0, time * -0.05)) * tiling);
    float n2 = tex2D(noiseTex2, (coords + float2(0.25, time * -0.06)) * tiling);
    float n3 = tex2D(noiseTex3, (coords + float2(0.5, time * -0.07)) * tiling);
    float noise = saturate((n1 + n2 + n3) / 2.0f);
    float3 noiseRgb = lerp(outerColor, innerColor, noise);
    
    float4 finalColor = float4(noiseRgb, mask);
    
 
    float4 glowingColor = float4(glowColor, mask);
    return (glowingColor + finalColor) * sampleColor;
}


technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};