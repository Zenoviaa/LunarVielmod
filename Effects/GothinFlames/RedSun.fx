sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float3 flameInsideColor;
float3 flameBloomColor;
float time;

float4 SpriteNoise(float2 coords, float dir)
{
    float2 uv = 2.2 * coords - 1.1;
    uv.x = acos(uv.x / cos(uv.y = asin(uv.y))) - time * 0.15 * dir;
    
    float2 scrollingCoords = uv;
    float4 spriteColor = tex2D(spriteSampler, frac(scrollingCoords * 0.75));
    return spriteColor;
}

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float2 polarUV = float2(angle, dist);
    return polarUV;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
   
    
    float noiseCoords = coords + float2(time * -0.05, time * 0.05);
    noiseCoords = frac(noiseCoords);
  //  noiseCoords = round(noiseCoords * 2.0) / 2.0;
    float n = tex2D(noiseSampler, noiseCoords).r;
    
    float2 diff = (coords - float2(0.5, 0.5));
    float len = length(diff);
    float d = len < 0.5;
      
    

    float radians = n * 4.5;
    float2 offset = float2(cos(radians), sin(radians)) * 0.06;
    
    float4 crystalColor = SpriteNoise(frac(coords + offset), 1.0);
    float4 crystalColor2 = SpriteNoise(frac(coords + offset), 0.5);
    float4 crystalColor3 = crystalColor + crystalColor2;
    //float e = n > 0.5;
    float3 flameColor = lerp(flameInsideColor, flameBloomColor, n);
  //  crystalColor *= d;
    float4 finalColor = crystalColor3 * sampleColor * 1.5 * n;
    float alphaFalloff = smoothstep(1.0, 0.0, pow(saturate(len / 0.5), 3.0));
    finalColor.rgb *= alphaFalloff;
    finalColor.rgb *= flameColor;
    finalColor.rgb *= 2.0;
    finalColor.rgb *= lerp(1.0, 2.0, n);
    finalColor.rgb *= lerp(1.0, 2.0, sin(time + coords.x) * 0.5 + 0.5);
    finalColor.rgb *= 0.25;
    
    float2 polarCoords = PolarCoordinates(coords * 0.4);
    polarCoords.y -= time * 0.2;
    float4 polarColor = tex2D(spriteSampler, polarCoords);
    finalColor.rgb -= polarColor.rgb * alphaFalloff;

    //finalColor.rgb *= e;

    float fade = saturate(len / 0.5);
 //   finalColor.rgb += (smoothstep(0.0, 1.0, fade) * (1.0 - fade));
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}