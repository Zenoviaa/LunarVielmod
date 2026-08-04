sampler uImage0 : register(s0);
float time;
float frequency;
float amplitude;
float3 bloomColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;

    uv.y += sin(time + coords.x * frequency) * amplitude;
    uv.x *= 5.0;
    
    uv.x += time;
    uv = frac(uv);
    
    float3 spriteRGB = tex2D(uImage0, uv).rgb;
    spriteRGB = lerp(spriteRGB, tintColor.rgb, coords.x);
    
    float yInterpolant = smoothstep(0.0, 1.0, abs(uv.y - 0.5) / 0.5);
    spriteRGB = lerp(spriteRGB, bloomColor, yInterpolant);
    spriteRGB = lerp(spriteRGB, float3(0.0, 0.0, 0.0), yInterpolant);
    spriteRGB = lerp(spriteRGB, float3(0.0, 0.0, 0.0), coords.x);
    float4 spriteColor = tex2D(uImage0, uv);
    float4 finalColor = float4(spriteRGB, tintColor.a);
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}