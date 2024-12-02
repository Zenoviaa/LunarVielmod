matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;
float time;
float repeats;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 st = float2(input.TextureCoordinates.x * repeats, 0.25 + input.TextureCoordinates.y * 0.5);

    float3 color = tex2D(primaryTex, st + float2(time, 0.0)).xyz;
    float3 color2 = tex2D(primaryTex, st + float2(-time * 1.5, 0.0)).xyz * 0.5;

    float sam = tex2D(noiseTex, float2(st.y, time)).x;
    float mult = tex2D(noiseTex, st).x + (input.Color.g * -2.0);

    float4 output = float4((color + color2) * input.Color.rgb * (1.0 + color.x * 2.0), color.x * input.Color.w);

    //if((input.Color.g) < mult)
    output *= sam + input.Color.g;

    return output;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}