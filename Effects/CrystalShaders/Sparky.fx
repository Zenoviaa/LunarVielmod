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
float2 tiling;
float3 innerColor;
float3 outerColor;
float distortion;

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
    float2 polarCoords = PolarCoordinates(coords);
    float2 noiseCoords = polarCoords + float2(time * -0.05, time * -0.05);
    float noise = tex2D(noiseTex, noiseCoords * tiling);

    //This should push the coordinates in that direction... right?
    //If this works it's gonna look sick
    float2 direction = coords;
    direction -= 0.5f;
    direction *= 2.0f;
    float2 distortedCoords = coords + direction * distortion * noise;
    float mask = tex2D(uImage0, distortedCoords);
    float3 rgb = lerp(outerColor, innerColor, mask);
    float4 finalColor = float4(rgb, mask) * sampleColor;
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};