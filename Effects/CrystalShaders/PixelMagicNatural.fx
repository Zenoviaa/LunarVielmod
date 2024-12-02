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

float3 primaryColor;
float3 noiseColor;

float distortion;
float threshold;

float3 LeavesPixelShaderFunction(float2 coords)
{
    //Calculate Distorting Noise
    float s = tex2D(uImage1, coords + float2(time * -0.5, 0.0)).r;
    
    //Multiply by coords.x to have back leaves distort more
    float offset = s * distortion * (1.0 - coords.x);
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    //Calculate Leaves
    float leavesSample = tex2D(uImage0, oCoords);
    float3 leavesColor = leavesSample * primaryColor;
    return leavesColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Normalized pixel coordinates (from 0 to 1)
    float4 sampleColor = tex2D(uImage0, coords);
    
    //Calculate Leaves
    float3 leavesColor = LeavesPixelShaderFunction(coords);
      
    //Scroll the noise
    float s = tex2D(uImage1, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    float shapeSample = tex2D(uImage2, oCoords);
    float3 shapeColor;
    if (shapeSample > threshold)
    {
        shapeColor = leavesColor * sampleColor.rgb;
    }
    else
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }

    // Output to screen
    float4 finalColor = float4(shapeColor, 1.0) * sampleColor.w;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}