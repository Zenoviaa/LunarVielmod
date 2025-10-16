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


float3 innerColor;
float3 outerColor;
float3 fadeColor;

float2 velocity;
float distortion;
float power;
float time;
float2 tiling;
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

float2 DistortCoordinates(float2 coords)
{
    float2 offsetCoords = coords + float2(0.0, time * -0.025);
    float sample = tex2D(noiseTex, offsetCoords * tiling);
    float rot = lerp(0, 3.14, sample);
    float2 angleOffset = float2(sin(rot), cos(rot)) * distortion;
    return coords + angleOffset;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Distort the coordinates for that wobbly effect
    coords = DistortCoordinates(coords);
    coords += velocity * (1.0 - coords.y);
    
    
    //We need to create like a base fill for the fire
    //So we'll sample the noise and power it so it's mostly white
    float2 noiseCoords = (coords + float2(0, time * 0.05)) * tiling;
    float fillNoiseSample = tex2D(noiseTex, noiseCoords);
        
    //Then we need to weaken the sample based on the y  coordinate, as it goes up,
    //So it like shrinks
    //Smoothstep so it's nice and interpolated
    float noisePow = smoothstep(2.0, power, coords.y);
    fillNoiseSample = pow(fillNoiseSample, noisePow);
    
    //Now we need another scrolling noise texture to add like interest
    float2 n2Coords = (coords + float2(0, time * 0.06) * tiling);
    float n2Sample = tex2D(noiseTex, n2Coords);
    n2Sample = pow(n2Sample, noisePow);
    
    //Then one more
    float2 n3Coords = (coords + float2(0, time * 0.07) * tiling);
    float n3Sample = tex2D(noiseTex, n3Coords);
    n3Sample = pow(n3Sample, noisePow);
    
    float noise = fillNoiseSample + n2Sample + n3Sample;
    noise = saturate(noise);
    
    //Now we calculate colors
    float3 fireColor = lerp(innerColor, outerColor, noise);
    fireColor = lerp(fadeColor, fireColor, coords.y);
    float3 noiseFireRGB = fireColor * noise;
    
    //Multiply with the mask/shaping texture
    float4 finalColor = tex2D(uImage0, coords);
    finalColor.rgb *= noiseFireRGB;
    return finalColor * sampleColor;
    
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};