
#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
float power;
float time;
float4 glowColor;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates.xy;
    coords.y += sin(coords.x * 3.0) * 0.3;
    float alpha = QuadraticBump(coords.x);
    float4 spriteColor = tex2D(spriteSampler, coords);
    spriteColor.rgb *= input.Color.rgb;
    spriteColor = pow(spriteColor, lerp(15.0, 0.8, input.Color.a));
    spriteColor *= 2.0;
    spriteColor += spriteColor * input.Color.a;
    spriteColor *= lerp(0.5, 1.0, sin(time * 4.0) * 0.5 + 0.5);
    spriteColor *= alpha;
    spriteColor *= input.Color.a;
    spriteColor.a = 0.0;
    return spriteColor;
    //return input.Color;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
