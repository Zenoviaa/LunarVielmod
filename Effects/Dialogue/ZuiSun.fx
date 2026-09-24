sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    coords = floor(coords * 128.0) / 128.0;
    coords.x += sin(coords.y * 64.0 + time * 4.0) * 0.01;
    
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    float fade = 1.0 - saturate(l / 0.5);
    float4 backglowColor = float4(1.0, 0.4, 0.4, 1.0);
    float scale = 1.1;
    backglowColor *= fade * 1.1;
    
    float4 outlineColor = float4(0.4, 0.7, 0.0, 1.0);
    if (l > 0.25)
        outlineColor *= 0.0;
    
    float4 middleColor = float4(1.0, 1.0, 1.0, 1.0);
    if (l > 0.22)
        middleColor *= 0.0;
    

    float4 finalColor = backglowColor + outlineColor + middleColor;

    finalColor *= 1.1;
    return finalColor * tintColor;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}