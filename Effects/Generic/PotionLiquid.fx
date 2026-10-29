sampler spriteSampler : register(s0);
sampler noiseSampler : register(s2);
float4 outsideColor;
float4 insideColor;
float4 outlineColor;
float2 metaballs[32];
float2 spriteSize;
float2 noiseTexelSize;
float metaballRadius;
float metaballStrength;
float time;

float DistanceSquared(float2 a, float2 b)
{
    float num = a.x - b.x;
    float num2 = a.y - b.y;
    return num * num + num2 * num2;
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 ditherCoords = coords * spriteSize * noiseTexelSize;
    float4 ditherColor = tex2D(noiseSampler, ditherCoords);
  
    
    float speed = time;
    speed *= 0.4;
    float2 noiseCoords1 = frac(coords + float2(
    sin(coords.y * 4.0 + speed * 1.5) * 0.04, speed * -0.15 + 0.2));
    
    float2 noiseCoords2 = frac(coords  + float2(
    sin(coords.y * 4.0 + speed * -1.5) * 0.04 + 0.8 + speed * 0.5, speed * 0.15));
    
    float pixelated = 128.0;
    coords = floor(coords * pixelated) / pixelated;
    noiseCoords1 = floor(noiseCoords1 * pixelated) / pixelated;
    noiseCoords2 = floor(noiseCoords2 * pixelated) / pixelated;

    
    //we can assume the metaballs are already in texture spac
    float totalContribution = 0.0;
    float metaballRadiusSquared = metaballRadius * metaballRadius;
    for (int i = 0; i < 32; i++)
    {
        float2 mb = metaballs[i];
        float distSquared = DistanceSquared(coords, mb);
        float influence = (metaballRadius / distSquared) * metaballStrength;
        influence += sin(time * 3.14 + coords.y * 8.0) * 0.025;
     //   influence += cos(time * 3.14 + coords.x * 6.0) * 0.05;
     
        float p = pow(influence, 1.4);
        totalContribution += p;
        
        

    }

    float l = totalContribution / 4.0;
    l = saturate(l);
    

    float4 noiseColor = tex2D(spriteSampler, noiseCoords1);
    
    float4 noiseColor2 = tex2D(spriteSampler, noiseCoords2);
    
    float4 noiseColor4 = tex2D(spriteSampler, frac(float2(coords.y, coords.x * 0.3) + float2(speed * 0.5, 0.0)));
    
    float4 noiseColor3 = noiseColor + noiseColor2;
   
    
    noiseColor3 = floor(noiseColor3 * 4.0) / 4.0;
    noiseColor3 *= pow(coords.y, 2.0);
    
    float invContribution = 1.0 - l;
    float glowInvContribution = 1.0 - l;
    invContribution = round(invContribution * 4.0) / 4.0;
    
    float glowingFactor = saturate(totalContribution / 3.0);
    float4 metaballColor = lerp(float4(0.0, 0.0, 0.0, 0.0), insideColor, l);
    //metaballColor *= 2.0;
  //  metaballColor = floor(metaballColor * 8.0) / 8.0;
    if(metaballColor.a < 0.82)
        metaballColor *= 0.0;
    metaballColor.rgb *= coords.y * metaballColor.a * 0.75;
    
    float4 oc = lerp(float4(1.0, 1.0, 1.0, 1.0), outlineColor, coords.y * 1.8);
    oc += sin(time + coords.x * 4.0) * 0.7;
    oc += cos(time + coords.y * 8.0) * 0.3;
    metaballColor += oc * invContribution * metaballColor.a * 5.0;
    //metaballColor.gb -= invContribution * metaballColor.a * 7.0;
    metaballColor += (tintColor * glowingFactor * 1.0 * (1.0 - metaballColor.a)) * coords.y * 0.5;
    metaballColor += noiseColor3 * metaballColor.a * tintColor * 0.7;
   
    float2 bottomDiff = (coords - float2(0.5, 1.0));
    float bottomL = length(bottomDiff);
    float d = 1.0 - saturate(bottomL / 0.55);
    float d2 = saturate(bottomL / 0.55);
    metaballColor.rg -= d * 0.5;
    metaballColor += tintColor * pow(d2, 3.0) * metaballColor.a * 0.2;
    metaballColor += ditherColor * 0.08 * metaballColor.a;
    metaballColor.b += coords.y * 0.03 * metaballColor.a;
    metaballColor += coords.y * metaballColor.a * 0.03;
    metaballColor += 0.0125 * metaballColor.a;
    metaballColor += noiseColor4 * metaballColor.a * tintColor * 0.5;
    metaballColor = floor(metaballColor * 8.0) / 8.0;
  
    //metaballColor = floor(metaballColor * 8.0) / 8.0;
    //metaballColor.bg += pow(glowInvContribution, 0.1) * metaballColor.a * 0.4;
    //metaballColor.rgb += (1.0 - coords.y + 0.05) * metaballColor.a;
    //metaballColor.rgb -= sin(frac(time * 0.2 + coords.y * 0.2) * 3.14);
    //float4  glossColor = pow(metaballColor, 4.0) / 4.0;
    //metaballColor += glossColor * 256.0;
    return metaballColor * 0.9;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}