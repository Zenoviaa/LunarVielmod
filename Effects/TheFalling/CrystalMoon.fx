sampler crystalSampler : register(s0);
sampler noiseSampler : register(s1);
float4 glowColor;;
float time;

float4 Crystals(float2 coords)
{       
    float2 uv = 2.2 * coords - 1.1;
    uv.x = acos(uv.x / cos(uv.y = asin(uv.y))) - time * 0.15;
    
    float2 scrollingCoords = uv;
    float4 crystalColor = tex2D(crystalSampler, frac(scrollingCoords * 2.0));
    return crystalColor;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{     
    float noiseCoords = coords + float2(time * -0.05, time * 0.05);
    noiseCoords = frac(noiseCoords);
    noiseCoords = round(noiseCoords * 2.0) / 2.0;
    float n = tex2D(noiseSampler, noiseCoords).r;
    
    float2 diff = (coords - float2(0.5, 0.5));
    float len = length(diff);
    float d = len < 0.5;
    
    
    float4 crystalColor = Crystals(coords);
    crystalColor *= d;
    float4 finalColor = crystalColor * sampleColor * 1.5 * n;
    
    
    finalColor.rgb *= smoothstep(1.0, 0.0, pow(saturate(len / 0.5), 3.0));
    
    float fade = saturate(len / 0.5);
    finalColor.rgb += (smoothstep(0.0, 1.0, fade) * (1.0 - fade));
  //  finalColor *= 2.0;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}