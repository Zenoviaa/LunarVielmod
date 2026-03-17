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

Texture2D GradientTexture;
sampler2D GradientTextureSampler = sampler_state
{
    Texture = <GradientTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};
Texture2D GradientBackTexture;
sampler2D GradientBackTextureSampler = sampler_state
{
    Texture = <GradientBackTexture>;
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
float Waviness;
float2 RTSize;
float2 Parallax;
float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}


float SampleRayNoise(float2 coords, float2 noiseOffset)
{
    float2 uv = coords;
    uv += Parallax;
    uv = frac(uv);
    
    float2 distortedCoords = uv;
    distortedCoords += float2(Time * -0.05, 0.0);
    distortedCoords = frac(distortedCoords);
    float n = tex2D(DistortionTextureSampler, distortedCoords);

    //Distort the coordinates to create that aurora shape
    float2 auroraCoords = uv;
    auroraCoords.x += sin(Time + uv.y * 8.0) * 0.02;
    float depth = uv.y * uv.y;
    auroraCoords.y += depth;
    auroraCoords.y *= 0.03;
    
    float osc = sin(Time + uv.x * 8.0) * 0.5 + 0.25;
    osc *= Waviness;
    auroraCoords.y += osc;
    auroraCoords += noiseOffset;
    auroraCoords = frac(auroraCoords);
    float noise = tex2D(uImage0, auroraCoords).r;
    
    //Create oscilliation in the colors
    float black = sin(Time + uv.x * 16.0) * 0.5 + 0.5;
    noise *= black;
    return noise;
}

float3 SampleRayColor(float2 uv, float depth)
{     
    //Hardcoded aurora colors for testing
    //May edit later or sample a gradient for animations
    const float3 auroraStart = float3(167.0 / 255.0, 241.0 / 255.0, 204.0 / 255.0);
    const float3 auroraEnd = float3(4.0 / 255.0, 75.0 / 255.0, 171.0 / 255.0);
    const float3 auroraBackEnd = float3(194.0 / 255.0, 91.0 / 255.0, 198.0 / 255.0);
    

    float2 gradientCoords = uv * 8.0;
    gradientCoords.x += Time * -0.1;
    gradientCoords = frac(gradientCoords);
    float3 gradientStartRGB = tex2D(GradientTextureSampler, gradientCoords).rgb;
    float3 gradientEndRGB = tex2D(GradientBackTextureSampler, gradientCoords).rgb;
    
    
    
    
    float3 mixedEnd = lerp(auroraEnd, auroraBackEnd, depth);
    float3 col = lerp(gradientStartRGB, gradientEndRGB, uv.y);
    return col;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;

    float depth = uv.y * uv.y; 
    float3 rayRGB = SampleRayColor(uv, depth);
    float rayNoise = SampleRayNoise(coords, float2(0.0, 0.0));
    float rayNoise2 = SampleRayNoise(coords, float2(-0.2, 0.3));
    float mixedRayNoise = rayNoise + rayNoise2;
    
    //Mix two noises to create cool movement within the aurora
    //We'll multiply the end result by 2 to give it some bloom
    float4 rayColor = float4(mixedRayNoise, mixedRayNoise, mixedRayNoise, 1.0);
    rayColor.rgb *= rayRGB;
    rayColor.rgb *= 2.0; 
    rayColor.rgb *= 1.0 + coords.y * coords.y;
        
    /*
    rayColor.rgb = ScreenSpaceDither(uv * RTSize, 5.0);
    float levels = 8.0;
    rayColor.r = posterize(rayColor.r, levels);
    rayColor.g = posterize(rayColor.g, levels);
    rayColor.b = posterize(rayColor.b, levels);*/

    //Calculate Edge Fades
    float rayOsc = sin(Time + uv.x * 8.0) * 0.3;
    float rayCos = cos(Time + uv.x * 8.0);
    float rayLerp = QuadraticBump(uv.y);
    float rayLerp2 = QuadraticBump(uv.y + rayOsc);
    rayColor *= rayLerp * rayLerp2;
    
   
    return rayColor * sampleColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};