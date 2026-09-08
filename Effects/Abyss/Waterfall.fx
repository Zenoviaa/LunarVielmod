sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float waveStrength;
Texture3D ColorSpectrumTexture;
sampler3D ColorSpectrumTextureSampler = sampler_state
{
    Texture = <ColorSpectrumTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

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
   // newCoords.x += sin(time * 4.0 + coords.x * 2.0) * waveStrength;
    float2 waterfallCoords = coords;
    waterfallCoords.x += sin(-t * 4.0 + coords.y * 8.0) * waveStrength;
  
 //   float x = sin(time * 4.0 + coords.x * 2.0) * waveStrength;
    float4 noiseColor = tex2D(noiseSampler, newCoords * 7.6 * float2(0.1, 0.3));
    noiseColor = lerp(noiseColor, float4(1.0, 1.0, 1.0, 1.0), noiseColor.r);
    noiseColor = floor(noiseColor * 12.0) / 12.0;
    float4 waterFallColor = tex2D(spriteSampler, waterfallCoords);
   
    float4 finalColor = waterFallColor * sampleColor * noiseColor * 1.9;
    finalColor.rgb *= 1.4;
    
    float osc = sin(coords.y * 14.0 + -t * 4.3) * 0.5 + 0.5;
    if(osc > 0.75)
        osc = 0.75;
    finalColor += osc * 0.4;
    finalColor += coords.y * 0.2;
    finalColor *= quadraticBump(coords.x);
    finalColor *= saturate(coords.y / 0.2);
    
    float4 colorToMapTo = tex3D(ColorSpectrumTextureSampler, finalColor.rgb);
    float4 newColor = finalColor;
    newColor.rgb = colorToMapTo.rgb * finalColor.a;
    
    float4 mixedColor = finalColor * 0.0 + newColor * 0.5;
    return mixedColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}