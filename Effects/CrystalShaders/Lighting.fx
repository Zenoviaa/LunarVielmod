sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
#define PI 3.1415926535897932
matrix transformMatrix;
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

//TODO: custom vertex structure for inputting light data
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 sampleColor = input.Color;
    float2 coords = input.TextureCoordinates;
    float2 vectorToPixel = coords - float2(0.5, 0.5);
    float pixelLength = length(vectorToPixel);
    float angle = (atan2(vectorToPixel.y, vectorToPixel.x) + PI) / (PI * 2.0);
    angle += 0.5;
    angle = frac(angle);
    
    float shadowMapY = sampleColor.a;
    
    //Angle is the x coordinate
    //Y is the index of the light
    //We should be able to render all the lights in 1 batch with this approach
    float2 shadowMapSampleCoord = float2(angle, shadowMapY);
    float distance = tex2D(uImage1, shadowMapSampleCoord).y;
    
    float falloff = 1.0;
    if (pixelLength > distance)
    {
        falloff -= (pixelLength - distance) / 0.05;
    }
 //   float falloff = lerp(1.0, 0.0, saturate(pixelLength / distance));
    float4 light = float4(1.0, 1.0, 1.0, 1.0);
    light *= falloff;
    return light;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};