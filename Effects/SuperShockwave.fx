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


float2 epicenter;
float radius;
float strength;
float interp;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{ 
    //Get the difference between the epicenter and current coords
    float2 diff = coords - epicenter;
    float2 uv = coords;
    float2 offset = float2(0.0, 0.0);
    //Calculate distance and check if it is within the shockwave radius
    float len = length(diff * float2(uScreenResolution.x / uScreenResolution.y, 1.));
    if (len < radius)
    {
        float2 dir = epicenter - uv;
        dir /= len;
        
        //Push more the closer it gets to the center of the hole
        float inverseLerp = 1.0 - (len / radius);
        offset = dir * strength * inverseLerp;
    }
    
    float2 changedUV = uv + offset;
    float2 finalUV = lerp(uv, changedUV, interp);

    float3 col = tex2D(uImage0, finalUV).rgb;
    return float4(col, 1.0);
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};