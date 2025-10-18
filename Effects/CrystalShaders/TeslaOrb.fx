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
float3 lightningInnerColor;
float3 lightningOuterColor;
float time;
float power;
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
    //SO
    //My idea to get this to work is to use polar coordinates with that water trail texture
    //and quadratic bump to get a donut shape and mask it
    //let's see if that works
    
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    //Let's try
    
    //Calculate the lightning
    float2 polarCoords = PolarCoordinates(coords);
    float2 lightningCoords = polarCoords + float2(time * 0.05, time * 0.05);
    float lightningSample = tex2D(noiseTex, lightningCoords);
    float3 lightningRgb = lerp(lightningOuterColor, lightningInnerColor, lightningSample);
    float4 lightningColor = float4(lightningRgb, 1.0);

    //Now a donut I think
    
    float mask = tex2D(uImage0, coords);
    float glow = pow(mask, power);

    float3 glowRgb = lerp(outerColor, innerColor, glow);
    float4 maskingColor = float4(glowRgb, 1.0) * glow;
    float4 finalColor = (maskingColor + (lightningColor * mask)) * sampleColor;
    finalColor = saturate(finalColor);
    return finalColor;
}


technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};