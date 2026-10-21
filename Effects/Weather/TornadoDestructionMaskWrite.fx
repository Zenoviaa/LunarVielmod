
sampler spriteSampler : register(s0);
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
    float4 spriteColor = tex2D(spriteSampler, input.TextureCoordinates.xy);
    //Now we need to multiply the alpha
    float offset = spriteSize.y * texelSize.y;
    
    float strength = offset * (1.0 - input.TextureCoordinates.y);
    spriteColor *= strength * 2.0;
    spriteColor *= input.Color;
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
