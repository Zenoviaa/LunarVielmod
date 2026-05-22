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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;

    // Time varying pixel color
    float3 col = tex2D(uImage0, uv).rgb;
    float3 black = float3(0.0, 0.0, 0.0);
    
    float leftStart = 0.52;
    float blackLerp = clamp(uv.x / leftStart, 0.0, 1.0);
    col = lerp(black, col, pow(blackLerp, 2.0));
    
    float topBottomEdge = 0.3;
    float blackLerp2 = clamp(uv.y / topBottomEdge, 0.0, 1.0);
    col = lerp(black, col, blackLerp2);
    
    
    float blackLerp3 = clamp((1.0 - uv.y) / topBottomEdge, 0.0, 1.0);
    col = lerp(black, col, blackLerp3);

    // Output to screen
    return float4(col, 1.0) * sampleColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};