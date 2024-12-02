matrix transformMatrix;
texture trailTexture;
texture secondaryTrailTexture;
texture tertiaryTrailTexture;
float4 primaryColor;
float4 secondaryColor;
float time;

sampler2D trailTex = sampler_state
{
    texture = <trailTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D secondaryTrailTex = sampler_state
{
    texture = <secondaryTrailTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D tertiaryTrailTex = sampler_state
{
    texture = <tertiaryTrailTexture>;
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
    float2 coords = input.TextureCoordinates;
    float2 offset = float2(time * -0.05, 0.0);
    float2 offset2 = float2(time * -0.025, 0.0);
    float2 offset3 = float2(time * -0.02, 0.0);
    
    //Sample the image
    float s = tex2D(trailTex, coords + offset);
    float s2 = tex2D(secondaryTrailTex, coords + offset2);
    float s3 = tex2D(tertiaryTrailTex, coords + offset3);

    float4 sampleColor = s * primaryColor;
    float4 sampleColor2 = s2 * secondaryColor;
    float4 finalColor = lerp(sampleColor, sampleColor2, s3);
    float mixedSample = s + s2;
    return mixedSample * finalColor * input.Color;

}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}