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

float2 lightingPos;
float lightRadius;
texture heightMap;
sampler2D heightMapSampler = sampler_state
{
    texture = <heightMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


bool OutOfBounds(float2 coords)
{
    if (coords.x < 0 || coords.x > 1.0 || coords.y < 0 || coords.y > 1.0)
        return true;
    return false;
}

float3 RayTrace(float2 coords, float2 dir)
{
        
    //So we cast a ray towards the light source
    //If it hits the height map then we don't light up
    float2 uvPos = coords;
    
    //if current position has a pixel return black
    float4 col = tex2D(heightMapSampler, uvPos).rgba;
    if (col.a > 0)
        return float3(0.0, 0.0, 0.0);
                     
    //Calculate aspect ratio, multiply by direction
    float2 aspectRatio = (uScreenResolution / uScreenResolution.y);
    dir = normalize(dir);
    dir *= aspectRatio;
    
    //We cast the ray
    const int STEPS = 16;
    
    [unroll]
    for (int n = 1; n < STEPS; n++)
    {
        //Sample from the height map
        //If something is hit, when trying to reach the light, then this pixel is not lit
        const float4 col = tex2D(heightMapSampler, uvPos).rgba;
        if (col.a > 0)
            return float3(0.0, 0.0, 0.0);

        //Otherwise we keep moving in that direction
        uvPos += dir;
        
        //If we leave the screen then stop
        if (OutOfBounds(uvPos))
            return float3(1.0, 1.0, 1.0);
    }
    
    return float3(1.0, 1.0, 1.0);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 lightingColor = float4(0.0, 0.0, 0.0, 1.0);
   
    //calculate direction to the light source
    float2 direction = (lightingPos - coords);
    float3 rayColor = RayTrace(coords, direction);
    float4 pixelColor = float4(rayColor, 1.0);
    return pixelColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};