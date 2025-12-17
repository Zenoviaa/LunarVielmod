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


texture maskTexture;
sampler2D maskTex = sampler_state
{
    texture = <maskTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 texelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    #define PI 3.14159
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;
    float4 mask = tex2D(maskTex, uv);
    float noise = mask.r;
   
    //We'll use a different shader to write these values properly
    //Green channel to store the rotaiton of the smear
    //Red channel will store the strength
    float normalAngle = mask.g;
    float inverseAngle = (normalAngle - 0.5) * 2.0 * PI;
    float2 velocity = float2(cos(inverseAngle), sin(inverseAngle));
    
    //uOpacity will store the strength
    velocity *= uOpacity;
    
    float2 distortedCoords = uv + velocity * noise * texelSize;
    float4 smearColor = tex2D(uImage0, distortedCoords);
    return smearColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};