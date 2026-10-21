
sampler spriteSampler : register(s0);
sampler tileSampler : register(s1);
float2 spriteSize;
float2 texelSize;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    
    float2 coords = input.TextureCoordinates.xy;
    
    //this tells how far to move down
    float maskColor = tex2D(spriteSampler, coords).a;
    float offsetCoords =  texelSize.y * 32.0 * maskColor;
    float4 spriteColor = tex2D(tileSampler, coords + offsetCoords);
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
