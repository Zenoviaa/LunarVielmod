matrix transformMatrix;
sampler uImage0 : register(s0);
float time;
float distortionStrength;

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
    coords.x *= 7;
    //Scroll the eyes and wiggle them up and down
    float2 offsetCoords = coords;
    offsetCoords.y += sin(time + coords.x * 4.0) * distortionStrength;
    offsetCoords.x += time * 0.05;
    offsetCoords.x = frac(offsetCoords.x);
    float4 trailColor = tex2D(uImage0, offsetCoords);
 
    //This should put a black drop shadow behind the eyes, we'll see if it works
    float blackAlpha = sin(coords.y * 3.14);
    float4 finalColor = float4(0.0, 0.0, 0.0, blackAlpha) + trailColor;
    finalColor.a = saturate(finalColor.a);
    return finalColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}