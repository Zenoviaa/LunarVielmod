sampler spriteSampler : register(s0);
float time;
float alpha;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 finalColor = tex2D(spriteSampler, coords);
    float4 originalColor = tex2D(spriteSampler, coords);
    finalColor.r = finalColor.g = finalColor.b = finalColor.a;
   // finalColor *=16.0;
    //finalColor /= 5.4;
    //finalColor.a = 0.0;

    return finalColor * sampleColor;

}

technique Technique1
{
    pass SpritePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}