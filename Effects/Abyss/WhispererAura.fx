sampler spriteSampler : register(s0);
float time;
float alpha;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 finalColor = tex2D(spriteSampler, coords);
    float4 originalColor = tex2D(spriteSampler, coords);
    for (float f = 0.0; f < 4.0; f++)
    {
        float2 offset = float2(cos(f * 6.28 + time), sin(f * 6.28 + time));
        float2 newCoords = coords + offset * 0.0005;
        float4 spriteColor = tex2D(spriteSampler, newCoords);
      
//        finalColor += spriteColor ;
    }
    finalColor *= 6.0;
    //finalColor /= 5.4;
    //finalColor.a = 0.0;
    
    float4 glowyFinalColor = lerp(finalColor, float4(1.0, 1.0, 1.0, 1.0), alpha * originalColor.a * 2.0);
    float4 myColor = lerp(glowyFinalColor, originalColor, alpha  );
    return myColor * sampleColor;

}

technique Technique1
{
    pass SpritePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}