
texture spriteTexture;
sampler2D spriteTex = sampler_state
{
    texture = <spriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

float4x4 projection;
float4 shadowColor;
float rotation;
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct Particle
{
    float4 InstancePosition : COLOR0;
};
    
struct VertexShaderOutput
{
    float4 Color : COLOR0;
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input, Particle p)
{
    VertexShaderOutput output;
    float4 vertexPosition = input.Position;
    vertexPosition.xy += p.InstancePosition.xy;

    output.Position = mul(vertexPosition, projection);
    output.Color = shadowColor;
    
    float2 texCoords = input.TextureCoordinates;
    output.TextureCoordinates = texCoords;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float alpha = input.TextureCoordinates.y;
    return input.Color * (1.0 - alpha);
}

technique SpriteDrawing
{
    pass PixelPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};