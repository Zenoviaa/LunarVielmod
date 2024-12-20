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
matrix uWorldViewProjection;
float4 uShaderSpecificData;
float uStretchReverseFactor;
float time;
float edgePower;
float progressPower;

float2 offset;
texture fogTexture;
sampler2D fogTex = sampler_state
{
    texture = <fogTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

// The X coordinate is the trail completion, the Y coordinate is the elevation on the point of the trail.
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float3 col = float3(0.0, 0.0, 0.0);
    const float increment = 0.2;
    float2 newCoords = coords + float2(time * -0.05, 0.0);
    newCoords += offset;
    //Combine the texture several times over, move in sin/cos for circle-ish blending
    for (float f = 0.0; f < 1.0; f += increment)
    {
        float p = f / 1.0;
        float rot = p * 6.28;
        float2 offsetCoords = coords + float2(
         sin(rot) * uTime,
         cos(rot) * uTime);
     
        float colorMap = tex2D(fogTex, offsetCoords).r;
        col += colorMap;
    }
    
    col /= (1.0 / increment);
    //col *= 2.0;

    // Fade out the Edges
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    l = pow(l, edgePower);
    if (l > 0.5)
    {
        l = 0.5;
    }
 
    float p = l / 0.5;
    p = pow(p, progressPower);
    if (p > 1.0)
    {
        p = 1.0;
    }
 
    // Output to screen
    float4 targetCol = float4(col, 1.0);
    targetCol.rgb *= sampleColor.rgb;
    float4 fragColor = targetCol * (1.0 - p);

    return fragColor * sampleColor.a;
}

technique Technique1
{
    pass PrimitivesPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}