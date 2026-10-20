
matrix transformMatrix;
sampler spriteSampler : register(s0);
float time;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;

};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;
  
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 position = input.Position;
    position.xz *= lerp(0.85, 1.0, sin(position.y * 5.0 + time * 5.0) * 0.5 + 0.5);
    position.xz -= lerp(0.0, 32.0, sin(position.y + time * 4.0) * 0.5 + 0.5);
//    position.x += lerp(32.0, 0.0, position.y / 128.0);
    output.Position = mul(position, transformMatrix);
    output.Normal = input.Normal;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates.xy;

    float4 spriteColor = tex2D(spriteSampler, frac(coords + float2(time * 0.5, time * -3.5)));
    float4 finalColor = input.Color * spriteColor * coords.y / 8.0;
    finalColor *= 1.8;
    finalColor *= input.Normal.r;

    //finalColor *= input.Normal.r;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
