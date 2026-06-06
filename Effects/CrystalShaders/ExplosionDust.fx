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
float2 parallax;
float xStretch;
float2 texelSize;
float2 SampleCoords(float2 uv, in float2 offset)
{
    uv.x *= xStretch;
    float2 coords = uv + parallax + offset;
    coords.x = frac(coords.x);
    coords.y = frac(coords.y);
    return coords;
}

float Sample(sampler textureSampler, float2 uv, in float2 offset)
{
    //Blurring so it's a bit smoother
    //Doing it in the texture loses some quality, in a shader it's perfect
    float2 leftCoords = uv + float2(-texelSize.x, 0.0);
    float2 rightCoords = uv + float2(texelSize.x, 0.0);
    float2 upCoords = uv + float2(0.0, -texelSize.y);
    float2 downCoords = uv + float2(0.0, texelSize.y);
    
    float left = tex2D(textureSampler, SampleCoords(leftCoords, offset)).r;
    float right = tex2D(textureSampler, SampleCoords(rightCoords, offset)).r;
    float up = tex2D(textureSampler, SampleCoords(upCoords, offset)).r;
    float down = tex2D(textureSampler, SampleCoords(downCoords, offset)).r;
    
    float avg = left + right + up + down;
    avg /= 4.0;
    return avg;

}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float cloudSample1 = Sample(uImage0, coords, float2(0.0, 0.0));
    float cloudSample2 = Sample(uImage1, coords, float2(time * -0.025, 0.2));
    float cloudSample3 = Sample(uImage2, coords, float2(time * -0.05, 0.4));
    float avgSample = cloudSample1 + cloudSample2 + cloudSample3;
    avgSample *= 0.4;
    
    float4 cloudColor = float4(avgSample, avgSample, avgSample, 1.0) * sampleColor;
    return cloudColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};