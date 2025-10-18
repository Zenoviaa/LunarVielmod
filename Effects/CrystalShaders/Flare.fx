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
float power;
float3 innerColor;
float3 outerColor;
float distortion;

texture distortionTexture;
sampler2D distortionTex = sampler_state
{
    texture = <distortionTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 DistortCoordinates(float2 coords)
{
    float n = tex2D(distortionTex, coords + float2(time * -0.1, 0.0));
    float2 distortedCoords = coords;
    distortedCoords.y += lerp(-1.0, 1.0, n) * distortion;
    distortedCoords.y = saturate(distortedCoords.y);
    return distortedCoords;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float2 distortedCoords = DistortCoordinates(coords);
    float mask = tex2D(uImage0, distortedCoords);
    mask = pow(mask, power);
    float3 rgb = lerp(outerColor, innerColor, mask);
    
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    float dist = length(float2(baseUV.x, baseUV.y));
    float alpha = smoothstep(1.0, 0.0, dist);
    float4 color = float4(rgb, mask) * sampleColor * alpha;
    return color;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};