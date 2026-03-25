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

Texture2D TrailTexture;
sampler2D TrailTextureSampler = sampler_state
{
    Texture = <TrailTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

Texture2D DistortionTexture;
sampler2D DistortionTextureSampler = sampler_state
{
    Texture = <DistortionTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};


float Time;
float Distortion;
float3 InnerColor;
float3 OuterColor;
float Bloom;
float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float2 noiseSampleCoords = uv + float2(Time * -0.05, 0.0);
    noiseSampleCoords = frac(noiseSampleCoords);
    float noiseSample = tex2D(DistortionTextureSampler, noiseSampleCoords).r;
    float noiseRadians = noiseSample * 3.14;
    float2 distortionOffset = float2(cos(noiseRadians), sin(noiseRadians)) * Distortion;
    
    //Soo basically
    //What if we sample two textures
    //We have a mask texture, and we sample the black/white 0-1 value
    //and use that as a y coordinate for the trailing texture
    //and scroll the trialing texture on the x axis
    
    float2 sampleUv = uv + distortionOffset;
    
    float n = tex2D(uImage0, sampleUv).r;
    float ySample = uv.x < 0.5 ? lerp(0.0, 0.5, n) : lerp(1.0, 0.5, n);
    float2 trailTextureCoords = float2(Time * -0.05, ySample);
    trailTextureCoords = frac(trailTextureCoords);
    float trailSample = tex2D(TrailTextureSampler, trailTextureCoords).r;
    float colorInterp = QuadraticBump(uv.y);
    float3 glowColor = lerp(OuterColor, InnerColor, colorInterp);
    float4 finalColor = float4(glowColor * trailSample, 1.0) * sampleColor;
    
    
    float4 maskColor = tex2D(uImage0, uv);
    float4 bloomColor = maskColor;
    bloomColor.rgb *= glowColor;
    bloomColor *= Bloom;
    bloomColor.a = 0;
    return finalColor + bloomColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};