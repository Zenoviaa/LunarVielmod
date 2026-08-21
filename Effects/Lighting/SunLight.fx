sampler heightMapSampler : register(s0);
float2 stepSize;
float shadowAlpha;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //This shader handles screenspace sun lighting by doing a simple raymarching over the tile target
    //If it encounters an opaque tile then it is in shadow
    //If it doesn't, then it's lit by sunlight
    const int MAX_STEPS = 48;

    float2 stepCoord = coords;
    stepCoord += stepSize * 2.0;
    float3 luminance = float3(1.0, 1.0, 1.0);
    float distanceTraveled = 0.0;
    float stepLength = length(stepSize);
    for (int i = 0.0; i < MAX_STEPS; i++)
    {
        float pixel = tex2D(heightMapSampler, stepCoord).a;
        if (pixel > 0)
        {
            float factor = 0.33 * (1.0 - saturate(distanceTraveled / 0.25));
            luminance.r -= factor;
            luminance.g -= factor;
            luminance.b -= factor * 0.75;
            break;
        }
       
        stepCoord += stepSize;
        distanceTraveled += stepLength;
        
        //If going out of bounds, just assume we're lit
        //Positive y does not need to be accomodated for
        if (stepCoord.x > 1 || stepCoord.y < 0 || stepCoord.x < 0)
        {
            break;
        }
    }
    
    float4 shadowedColor = float4(luminance, 1.0) * sampleColor;
    float4 sunColor = float4(1.0, 1.0, 1.0, 1.0) * sampleColor;
    return lerp(sunColor, shadowedColor, shadowAlpha);
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}