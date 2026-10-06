sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float distortionStrength;
float2 texelSize;
float2 noiseTexelSize;
float2 screenOffset;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 noiseCoords2 = coords + float2(time * 0.025, time * 0.05);
    noiseCoords2 += screenOffset;
    noiseCoords2 = frac(noiseCoords2);
    float noise2 = tex2D(noiseSampler, noiseCoords2).r;
    noise2 *= 0.2;
    
    float2 noiseCoords = coords + float2(time * -0.05, time * -0.025);
    noiseCoords += screenOffset;
    noiseCoords = frac(noiseCoords);
    float noise = tex2D(noiseSampler, noiseCoords).r;
    
    //Create distortions in the sprite
    float2 spriteSize = float2(1.0, 1.0) / texelSize;
    float2 distortionOffset = float2(0.0, distortionStrength * noise);
    distortionOffset *= spriteSize * noiseTexelSize;
    
    //Create distortions in the color
    float2 spriteCoords = coords + distortionOffset;
    float4 spriteColor = tex2D(spriteSampler, spriteCoords);
    spriteColor += noise2 * spriteColor.a;
   
    //Flicker brightly
    float brightness = sin(time * 3.0) * 0.5 + 0.5;
    brightness = lerp(brightness, 0.75, 1.7);
    spriteColor *= brightness;
    spriteColor.gb += spriteColor.a * 0.2;
    spriteColor.g *= 0.8;
    spriteColor.b *= 1.2;
    
    if (spriteColor.a <= 0.0)
    {
        for (int i = -1.0; i <= 1.0; i++)
        {
            for (int j = -1.0; j <= 1.0; j++)
            {
                float2 offset = float2(i, j);
                offset *= texelSize * 2.0;
                float4 col = tex2D(spriteSampler, coords + distortionOffset+ offset);
                if (col.a > 0.0)
                {
                    spriteColor.rgb += 1.0 * sin(time * 4.0) * 0.5 + 0.5;
                    spriteColor.rgb += 0.3;
                }

            }

        }
    }

        return spriteColor * sampleColor;

}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};