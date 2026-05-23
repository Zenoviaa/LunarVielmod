matrix transformMatrix;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 Color2 : COLOR1;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float4 Color2 : COLOR1;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.Color2 = input.Color2;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    
    const float maxDistance = length(float2(0.5, 0.5));
    float distanceFromCenter = length(uv - float2(0.5, 0.5));
    float interp = saturate(distanceFromCenter / 0.4);
    float alphaFactor = smoothstep(1.0, 0.0, interp);
    
    float3 glowingColor = lerp(input.Color2.rgb, input.Color.rgb, alphaFactor);
    float3 mixedColor = lerp(float3(0.0, 0.0, 0.0), glowingColor, alphaFactor);    
    return float4(mixedColor, 0.0);
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}