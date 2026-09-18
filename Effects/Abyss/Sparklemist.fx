sampler spriteSampler : register(s0);
float time;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    dist -= time * 0.2;
    dist = frac(dist);
    float2 polarUV = float2(angle, dist);
    return polarUV;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 polarUV = PolarCoordinates(coords );
   
    float2 scaledCoords = coords * 0.6;
    float2 noiseCoords1 = scaledCoords + float2(time * 0.2, 0.0);
    noiseCoords1 = frac(noiseCoords1);
    
    float2 noiseCoords2 = scaledCoords + float2(time * 0.1, 0.3);
    noiseCoords2 = frac(noiseCoords2);
    
    float4 mistColor = tex2D(spriteSampler, noiseCoords1);
    float4 mistColor2 = tex2D(spriteSampler, noiseCoords2);
    float4 mixedMistColor = mistColor + mistColor2;
    mixedMistColor /= 2.0;
    
    float fadeOut = 1.0 - saturate(length((coords - float2(0.5, 0.5))) / 0.5);
    float4 finalColor = mixedMistColor * fadeOut * sampleColor;
    finalColor *= 0.6;
    finalColor.a = 0.0;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}