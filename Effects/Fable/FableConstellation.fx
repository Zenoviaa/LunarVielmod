#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{ 
    float4 color = tex2D(spriteSampler, coords) * tintColor;

    color += QuadraticBump(coords.y) * 0.3;
    color *= sin(time * 16.0 + coords.y * 8.0);
    color += (sin(time * 16.0) * 0.5 + 0.5) * 0.2;
    color = pow(color, 2.0);
    return color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}