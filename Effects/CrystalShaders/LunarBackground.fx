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

float2 parallax[3];
float4 fadeToColor;
float time;
texture dustTexture;
sampler2D dustTex = sampler_state
{
    texture = <dustTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};
float4 SampleParallaxing(sampler textureSampler, float2 coords, float2 parallax, float depth)
{
    float2 offsetCoords = coords + parallax;
    //If going into the sky
    if(offsetCoords.y < 0.0)
        return float4(0.0, 0.0, 0.0, 0.0);

    float2 normalCoords = float2(frac(offsetCoords.x), frac(offsetCoords.y));
    float4 backgroundColor = tex2D(textureSampler, normalCoords);
    
    float yDepth = coords.y * depth * fadeToColor.a;
   
    normalCoords.x += time;
    normalCoords.x = frac(normalCoords.x);
    float disturbance = tex2D(dustTex, normalCoords).r;
    yDepth *= lerp(0.5, 1.0, disturbance);
    backgroundColor.rgb = lerp(backgroundColor.rgb, fadeToColor.rgb, yDepth);
    return backgroundColor;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 undergroundCoords = coords + parallax[2];
    if (undergroundCoords.y > 1.0)
    {
        float2 loopCoords = float2(frac(undergroundCoords.x), frac(undergroundCoords.y));
        float4 backgroundColor = tex2D(uImage3, loopCoords);
        
        
        float2 normalCoords = loopCoords;
        normalCoords.x += time;
        normalCoords.x = frac(normalCoords.x);

        float disturbance = tex2D(dustTex, normalCoords).r;
        float yDepth = 0.33;
        yDepth *= lerp(0.5, 1.0, disturbance);
        backgroundColor.rgb = lerp(backgroundColor.rgb, fadeToColor.rgb, yDepth);
        return backgroundColor * sampleColor;
    }
    
    //Using a matrix here just to store multiple values in the same variable
    float4 farLayer = SampleParallaxing(uImage2, coords, parallax[0], 1.0);
    float4 midLayer = SampleParallaxing(uImage1, coords, parallax[1], 0.66);
    float4 closeLayer = SampleParallaxing(uImage0, coords, parallax[2], 0.33);
    
    farLayer *= (1.0 - closeLayer.a) * (1.0 - midLayer.a);
    midLayer *= (1.0 - closeLayer.a);
    return (farLayer + midLayer + closeLayer) * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};