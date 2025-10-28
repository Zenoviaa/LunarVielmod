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
float3 innerRingColor;
float3 outerRingColor;


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


float4 Rings(float2 coords, float2 baseUV)
{
    //Need polar coords
    //Well
    //Not really actually
    //wait no we do
    #define PI 3.14159
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(baseUV);
    
    float2 polarUV = float2(angle, dist);
    
    //Calculate offset
    float2 offset = float2(-0.05f * time, -0.05f * time);
    polarUV += offset;
    
    //We'll use this to distort the colors
    float noise = tex2D(noiseTex, polarUV);
    float n = saturate(noise);
    
    float3 ringColor = lerp(outerRingColor, innerRingColor, n);

    float4 finalRingColor = float4(ringColor, 1.0);
    return finalRingColor;
}


float3 Black(float2 coords, float2 baseUV)
{
    float dist = length(baseUV);
    float3 blackRGB = float3(0.0, 0.0, 0.0);
    float3 whiteRGB = float3(1.0, 1.0, 1.0);
     
    //This should create a black circle at the center that blends into the surrounding color
    float interpolant = saturate((dist - 0.5) / 0.5);
    float3 blackColor = lerp(whiteRGB, blackRGB, interpolant);
    return blackColor;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    //Alright, so the challenge is to make a singularity shader
    //Not exactly sure how we're going to do this
    //But let's start with the rings first i guess?
    float4 ringColor = Rings(coords, baseUV);
    float3 blackColor = Black(coords, baseUV);
    float4 finalColor = ringColor * sampleColor;
    finalColor.rgb -= blackColor;
    
    
    //Fade the edges
    float dist = length(baseUV);
    float alpha = smoothstep(1.0f, 0.0f, saturate((dist - 0.75) / 0.25));
    finalColor *= alpha;
    
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};