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
        float du = distance / (1 - distance / (radius * radius - 1));
        float denom = du / radius + 1;
        
        //The attenuation is the falloff of the light depending on distance basically
        float attenuation = 1 / (denom * denom);
        diffuseLight.rgb += lightColor * lightIntensity * attenuation;
        diffuseLight *= attenuation;
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