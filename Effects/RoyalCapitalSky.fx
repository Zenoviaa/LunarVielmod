sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float uTime;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    //Calculate Distortion
    float distortionNoise = tex2D(noiseSampler, coords + uTime * 0.025).r;
    float distortion = 0.1;
    
    //Scrolling Noise
    float rot = distortionNoise * 4.142;
    float2 scrollingNoise = coords + float2(sin(rot) * distortion, uTime * 0.15);
    
    //Scroll multiple directions and lerp together to make a cool effect
    float3 col = tex2D(noiseSampler, coords + scrollingNoise).rgb * 0.5;
    for (float f = 0.0; f < 1.0; f += 0.25)
    {
        float rot = f * 3.14;
        float speed = 0.2;
        float2 offsetCoords = coords + float2(
            sin(rot) * uTime * speed,
            cos(rot) * uTime * speed);
                           
        float colorMap = tex2D(noiseSampler, offsetCoords + scrollingNoise).r;
        float3 targetCol = lerp(float3(0.333, 0.063, 0.518), float3(0.204, 0.329, 0.718), colorMap);
        float4 s = tex2D(noiseSampler, offsetCoords + scrollingNoise);
        col = lerp(col, targetCol, 0.25);
    }
    float4 fragColor = float4(col, 1.0);
    return fragColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};