matrix transformMatrix;
texture trailTexture;
sampler2D trailTex = sampler_state
{
    texture = <trailTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};
texture HighlightTexture;
sampler2D HighlightTex = sampler_state
{
    texture = <HighlightTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};
texture WindTexture;
sampler2D WindTex = sampler_state
{
    texture = <WindTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};

texture RimHighlightTexture;
sampler2D RimHighlightTex = sampler_state
{
    texture = <RimHighlightTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};

float2 Offset;
float2 Tiling;

float4 BaseColor;
float4 HighlightColor;
float4 RimHighlightColor;
float4 WindColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    //We have to multiply the position by the matrix so it appears in the correct spot
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 uv = input.TextureCoordinates;
    uv *= Tiling;
    uv += Offset;
    //Sample the image
    
    float4 s = tex2D(trailTex, uv) * BaseColor;
    float4 s2 = tex2D(HighlightTex, uv) * HighlightColor;
    float4 s3 = tex2D(WindTex, uv) * WindColor;
    float4 s4 = tex2D(RimHighlightTex, uv) * RimHighlightColor;
    return (s + s2 + s3 + s4) * input.Color;
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}