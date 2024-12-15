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
float size;
float pixelation;
float basePower;
float3 innerColor;
float3 glowColor;
float3 outerGlowColor;
float outerPower;
float height;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Making the circle
    coords = round(coords / pixelation) * pixelation;

    float start = 1.0 - height;
    float power = basePower + (sin(time) * 0.33);
    power = clamp(power, 0.0, 100.0);
    
    float wProgress = clamp(coords.y - start, 0.0, 1.0);
    float width = lerp(0.0, size, wProgress);
    float xDist = abs(coords.x - 0.5);
  
    float4 finalColor = float4(1.0, 1.0, 1.0, 1.0);
    if (xDist < width)
    {
        //Okay uh
        //We make circle
        //Set inner color
        finalColor.rgb = innerColor;

        //Calculate the glowing
        float totalProgress = xDist / width;
        if (totalProgress < 0.5)
        {
            float progress = totalProgress / 0.5;
            float3 col = lerp(innerColor, glowColor, progress);
            finalColor.rgb = col;
        }
        else
        {
            float progress = (totalProgress - 0.5) / 0.5;
            float3 col = lerp(glowColor, outerGlowColor, progress);
            finalColor.rgb = col;
        }
    
 
         //Then we calculate the fade out as it gets weaker
         //Tbf we could also additive draw
        float alphaProgress = xDist / width;
        float eased = pow(2.0, (outerPower * alphaProgress - outerPower));
        float mult = lerp(1.0, 0.0, eased);
        finalColor *= mult;
    }
    else
    {
        finalColor = float4(0.0, 0.0, 0.0, 0.0);
    }
    
    return finalColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};