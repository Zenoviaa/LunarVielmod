
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


float4 metaballs[32];
float3 innerColor;
float3 outerColor;
int length;
        
float2 texelSize;
float InExpo(float t)
{
    const float p = 10.0;
    return t == 0 ? 0 : pow(2, p * t - p);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float2 fixer = float2(texelSize.x / texelSize.y, 1.0);
    uv *= fixer;
    //Max number of metaballs we're sampling
    float totalContribution = 0.0;
    float radius = 0.06;
    
    //Going to try drawing this in screenspace, single pass on a render target
    for (int i = 0; i < length; i++)
    {
        float4 metaball = metaballs[i];
        float r = distance(uv, metaball.xy * fixer);
        float contribution = lerp(1.0, 0.0, saturate(r / metaball.w)) * metaball.z ;
        totalContribution += contribution;
    }
    
    //Clamp to 1.0 so it doesn't get mega bright
    //Then we're gonna interp colors and fade it out on the edges
    totalContribution = saturate(totalContribution);
    float3 metaballColorInner = lerp(outerColor, innerColor, InExpo(totalContribution));
    float3 metaballColorOuter = lerp(float3(0.2, 0.2, 0.2), float3(0.0, 0.0, 0.0), InExpo(totalContribution));
    float3 metaballColor = lerp(metaballColorOuter, metaballColorInner, totalContribution);
    float4 finalColor = float4(metaballColor, 1.0) * sampleColor * totalContribution;
    //float4 spriteColor = tex2D(uImage0, coords) * finalColor;
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};