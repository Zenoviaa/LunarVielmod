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



texture2D rippleTexture;
sampler2D rippleTex = sampler_state
{
    texture = <rippleTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


float4 SampleRippleByTexture(in float2 coords, in float4 sampleColor)
{
    float4 ripplerColor = tex2D(rippleTex, coords);
    //Now we need to decompress the info
    float normalAngle = ripplerColor.r;
    float strength = ripplerColor.g;
    float actualAngle = normalAngle * 6.28;
    float2 offset = float2(cos(actualAngle), sin(actualAngle)) * sin(strength * 0.2);
    float4 screenColor = tex2D(uImage0, coords + offset) * sampleColor;
    return screenColor;

}


float4 ripples[8];
int rippleLength;
float2 texelSize;
float4 SampleRippleByPoints(in float2 coords, in float4 sampleColor)
{
    float2 finalCoords = coords;
    for (int i = 0; i < rippleLength; i++)
    {
        float2 pos = ripples[i].xy;
        float radius = ripples[i].z;
        float strength = ripples[i].w;
        
        float2 vec = pos - coords;
        float dist = length(vec);
        float influence = 1.0 - saturate(dist / radius);
        influence *= strength;
        finalCoords += vec * texelSize * influence;
    }
    float4 screenColor = tex2D(uImage0, finalCoords);
    return screenColor;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    return SampleRippleByPoints(coords, sampleColor);

}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}