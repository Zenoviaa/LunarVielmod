#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float4 spriteColor = tex2D(spriteSampler, coords);
    float d = length(coords - float2(0.0, 0.5));
    float divisor = lerp(2.0, 0.5, tintColor.a * 4.0);
    float alpha = saturate(d / divisor);
    alpha = pow(alpha, 3.0);

    float osc = sin(time * 64.0) * 0.5 + 0.5;
    float osc2 = lerp(0.6, 1.0, osc);
    float4 finalColor = spriteColor * osc2 * 1.4;
    finalColor.rgb *= tintColor.rgb;
    finalColor *= 2.0;
    return finalColor * alpha  ;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}