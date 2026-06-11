sampler uImage0 : register(s0);
float2 texelSize;
float4 outlineColor;
float levels;
float4 SampleMixedColor(float2 coords)
{
    
    float2 leftCoords = coords + float2(-texelSize.x, 0.0);
    float2 rightCoords = coords + float2(texelSize.x, 0.0);
    float2 upCoords = coords + float2(0.0, -texelSize.y);
    float2 downCoords = coords + float2(0.0, texelSize.y);
 
    
    float4 left = tex2D(uImage0, leftCoords);
    float4 right = tex2D(uImage0, rightCoords);
    float4 up = tex2D(uImage0, upCoords);
    float4 down = tex2D(uImage0, downCoords);
    
    float4 colorToMix = tex2D(uImage0, coords);
    
    
    float4 avgColor = (left + right + up + down) / 4.0;
    avgColor = round(avgColor * levels) / levels;
    colorToMix = round(colorToMix * levels) / levels;
    
    
    float r = abs(avgColor.r - colorToMix.r);
    float g = abs(avgColor.g - colorToMix.g);
    float b = abs(avgColor.b - colorToMix.b);
    float diff = r + g + b;
    float d = diff > 0.06;
    return colorToMix * (1.0 - d) + (outlineColor * d * colorToMix.a);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    return SampleMixedColor(coords);
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}