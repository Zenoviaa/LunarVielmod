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

float lightRadius;
float2 lightingPos;
texture tileTexture;
sampler2D tileTex = sampler_state
{
    texture = <tileTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float lengthSquared(float2 v1)
{
    return v1.x * v1.x + v1.y * v1.y;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Lighting pos should already be converted to screen space by the cpu
    //So we can just do this
    float3 lightColor = sampleColor.rgb;
    float lightIntensity = sampleColor.a;
    
    
    float4 diffuseLight = float4(0.0, 0.0, 0.0, 1.0);
    float2 lightDirection = (coords - lightingPos) * (uScreenResolution / uScreenResolution.y);
    float distanceSq = lengthSquared(lightDirection);
    float radius = lightRadius;
    float radiusSquared = radius * radius;
    if (distanceSq < radiusSquared)
    {
        float distance = sqrt(distanceSq);
        float attenuation = 1.0f - (distance / (lightRadius / 2.0f));
        diffuseLight.rgb += lightColor * lightIntensity * attenuation;
        diffuseLight *= attenuation;
        float4 height = tex2D(tileTex, coords);
        if (height.a > 0.0)
        {
            diffuseLight *= attenuation * attenuation;
        }
    }
    return diffuseLight;
    
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};