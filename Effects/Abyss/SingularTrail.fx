sampler laserSampler : register(s0);
matrix transformMatrix;
float3 insideColor;
float3 bloomColor;
float time;

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
    float2 coords = input.TextureCoordinates;
    //Create black inside
    float diff = abs(coords.y - 0.5);
    float yInterpolant = 1.0 - saturate(diff / 0.5);
    float4 blackColor = float4(0.0, 0.0, 0.0, yInterpolant);
    float3 glowColor = lerp(insideColor, bloomColor, yInterpolant);
    float4 finalColor = blackColor;
    finalColor.rgb += glowColor * yInterpolant;
    
    float4 laserColor = tex2D(laserSampler, coords);
    float4 bigColor = finalColor * input.Color * 1.5;
    bigColor = pow(bigColor, 2.0);
    return bigColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}