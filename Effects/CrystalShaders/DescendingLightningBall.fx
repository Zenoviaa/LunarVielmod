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

float time;

float3 gradientStartColor;
float3 gradientMidColor;
float3 gradientEndColor;

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

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}

float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float3 power(in float3 color, float factor)
{
    color.r = pow(color.r, factor);
    color.g = pow(color.g, factor);
    color.b = pow(color.b, factor);
    return color;
}

float3 posterize(in float3 color, float factor)
{
    color.r = posterize(color.r, factor);
    color.g = posterize(color.g, factor);
    color.b = posterize(color.b, factor);
    return color;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
     // Normalized pixel coordinates (from 0 to 1)
    
    
    //Caclulate fading
    const float maxDistance = 0.5;
    float distanceFromCenter = length(uv - float2(0.5, 0.5));
    float interp = saturate(distanceFromCenter / maxDistance);
    float alphaFactor = smoothstep(1.0, 0.0, interp);
    
    float powerFactor = lerp(0.2, 2.0, interp);

    
    const float scrollSpeed = 0.5;
    float2 scrollingOffset = float2(time * scrollSpeed, time * -scrollSpeed);
    float3 sampleColor1 = tex2D(noiseTex, uv + scrollingOffset).rgb;
   
       
    float2 scrollingOffset2 = float2(time * -scrollSpeed, time * -scrollSpeed);
    float3 sampleColor2 = tex2D(noiseTex, uv + scrollingOffset2).rgb;
    
    
    //Blow up the size of the texture based on distance from center
    sampleColor1 = power(sampleColor1, powerFactor);
    sampleColor2 = power(sampleColor2, powerFactor);
    
    //Create a basic gradient to lerp between
    float3 gradientColor1 = lerp(gradientStartColor, gradientMidColor, interp);
    float3 gradientColor2 = lerp(gradientMidColor, gradientEndColor, interp);
    float3 gradientColor = lerp(gradientColor1, gradientColor2, interp);
    
    float3 mixedColor = lerp(sampleColor1, sampleColor2, 0.5);
    mixedColor.rgb = lerp(mixedColor.rgb, gradientColor, interp);
    
    float levels = lerp(8.0, 2.0, interp);
    mixedColor = posterize(mixedColor, levels);
    
    //Use less colors as we get further out from the center
  
    float3 combinedColor = (gradientColor * alphaFactor) + (mixedColor * alphaFactor);

    return float4(combinedColor, 0.0);
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};