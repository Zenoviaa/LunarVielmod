sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float waveStrength;
float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float t = time;
    t += sampleColor.a * 8.0;
    float2 newCoords = coords + float2(0.0, -t + coords.y * 0.2);
    //newCoords.x += sin(time * 4.0 + coords.x * 2.0) * waveStrength;
    float2 waterfallCoords = coords;
    waterfallCoords.x += sin(-t * 4.0 + coords.y * 32.0) * waveStrength;
    
    float4 noiseColor = tex2D(noiseSampler, newCoords * 1.6 * float2(0.1, 0.3));
    noiseColor = lerp(noiseColor, float4(1.0, 1.0, 1.0, 1.0), noiseColor.r);
    noiseColor = floor(noiseColor * 12.0) / 12.0;
    float4 waterFallColor = tex2D(spriteSampler, waterfallCoords);
    float4 finalColor = waterFallColor * sampleColor * noiseColor * 1.9;
    finalColor.rgb *= 1.4;
    
    float osc = sin(coords.y * 14.0 + -t * 4.3) * 0.5 + 0.5;
    finalColor += osc * 0.4;
    finalColor += coords.y * 0.2;
    finalColor *= quadraticBump(coords.x);
    finalColor *= saturate(coords.y / 0.2);
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}