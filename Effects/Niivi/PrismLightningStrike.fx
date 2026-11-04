#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float bounceTime;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float hueOffset = tintColor.b;
    float alpha = tintColor.a;
    float4 spriteColor = tex2D(spriteSampler, coords);
    float4 originalColor = spriteColor;
    float4 prismaticColor = float4(1.0, 0.0, 0.0, 1.0);
    prismaticColor.rgb = ApplyHue(prismaticColor.rgb, time * -12.0 + coords.x * 12.0 * spriteColor.r * 0.35 + hueOffset * 10.0);
    float osc = sin(time * 36.0) * 0.5 + 1.0;
    
    spriteColor *= osc;
    spriteColor *= prismaticColor;
    spriteColor = floor(spriteColor * 16.0) / 16.0;
    spriteColor *= 1.8;
    spriteColor += 1.0 * coords.y * spriteColor.r;
    spriteColor = pow(spriteColor, 0.8);
    return (spriteColor + originalColor * 0.6) * alpha;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}