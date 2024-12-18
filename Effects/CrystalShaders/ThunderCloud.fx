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
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

texture cloudTexture;
float3 cloudColor;
float distortion;
float pixelation;
float2 sourceSize;
float2 cloudTexSize;
sampler2D cloudTex = sampler_state
{
    texture = <cloudTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
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
float colorDistance2(float3 a, float3 b)
{
    float ar = abs(b.r - a.r);
    float ag = abs(b.g - a.g);
    float ab = abs(b.b - a.b);
    float d = ar + ag + ab;
    return d;
}

float3 calculateColor(float3 color)
{
	// Palette 1
    const float3 colors[5] =
    {
        float3(0, 0, 0),
        float3(0.25, 0.25, 0.25),
        float3(0.5, 0.5, 0.5),
        float3(0.75, 0.75, 0.75),
        float3(1.0, 1.0, 1.0),
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 5; i++)
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



float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 noiseCoords = (coords * sourceSize - uSourceRect.xy) / cloudTexSize;
    float noise1 = tex2D(cloudTex, noiseCoords + float2(uTime * 0.05, 0.0));
    float noise2 = tex2D(cloudTex, noiseCoords + float2(uTime * 0.025, 0.02));
    float noise3 = tex2D(cloudTex, noiseCoords + float2(uTime * 0.015, -0.02));
    
    


    
    float final = (noise1 + noise2 + noise3) * 0.4;
    
    float2 finalCoords = noiseCoords + float2(uTime * 0.03f, 0.01);
    finalCoords = round(finalCoords / pixelation) * pixelation;
    
    float4 color = tex2D(cloudTex, finalCoords) * final;
    color.rgb *= cloudColor;

    float distortionSample = tex2D(noiseTex, noiseCoords + float2(uTime * 0.05, 0.02));
    float2 coordsOffset = float2(uTime * 0.05, uTime * 0.02) * distortionSample * distortion;
    
    
    float4 originalColor = tex2D(uImage0, coords + coordsOffset);
    originalColor.rgb *= color.rgb;
    originalColor.rgb = calculateColor(originalColor.rgb);
    return originalColor;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}