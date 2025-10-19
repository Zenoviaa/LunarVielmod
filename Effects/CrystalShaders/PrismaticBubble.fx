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
float2 tiling;
float3 outerColor;
float distortion;

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

float3 HueShift(float3 color, float hueAdjust)
{
    const float3 k = float3(0.57735, 0.57735, 0.57735);
    float cosAngle = cos(hueAdjust);
    return color * cosAngle + cross(k, color) * sin(hueAdjust) + k * dot(k, color) * (1.0 - cosAngle);
}
float colorDistance2(float3 a, float3 b)
{
    float ar = abs(b.r - a.r);
    float ag = abs(b.g - a.g);
    float ab = abs(b.b - a.b);
    float d = ar + ag + ab;
    return d;
}

float3 Palettize(float3 color)
{
    	// Palette 1
    const float3 colors[7] =
    {
        float3(1.0, 0.0, 0.0),
        float3(1.0, 0.5, 0.0),
        float3(1.0, 1.0, 0.0),
        float3(0.0, 1.0, 0.0),
        float3(0.0, 1.0, 1.0),
        float3(0.0, 0.0, 1.0),
        float3(1.0, 0.0, 1.0),
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 7; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        if (currentDist < dist)
        {
            dist = currentDist;
            selectedColor = colors[i];
        }
    }
    float3 finalColor = lerp(color, selectedColor, uProgress);
    return finalColor;
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float n = tex2D(noiseTex, (coords + float2(0.0, time * -0.1)) * tiling);
    float2 distortedCoords = coords;
    distortedCoords.x += lerp(-1.0, 1.0, n) * distortion;
    distortedCoords.x = saturate(distortedCoords.x);
    
    
    #define PI 3.14159
    float3 startingColor = float3(1.0, 0.0, 0.0);
    float3 shiftedColor = HueShift(startingColor, n * PI + time);
    //So we gotta take the noise
    //Create a 
    
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float mask = tex2D(uImage0, distortedCoords);
    mask = pow(mask, power);

    float4 finalColor = float4(shiftedColor, mask) * sampleColor;
    finalColor.rgb = Palettize(finalColor.rgb);
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};