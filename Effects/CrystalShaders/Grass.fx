matrix transformMatrix;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    //We have to multiply the position by the matrix so it appears in the correct spot
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    return output;
}

float Posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float3 Posterize(in float3 color, float factor)
{
    color.r = Posterize(color.r, factor);
    color.g = Posterize(color.g, factor);
    color.b = Posterize(color.b, factor);
    return color;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    const float levels = 8.0;
    float4 color = input.Color;
    color.rgb = Posterize(color.rgb, levels);
    return color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}