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
    coords += float2(time * -0.05, 0.0);
    coords = frac(coords);
    
    //Applying the same thing here, only going to draw above a threshold and have it scale with the  x coordinate
    //This should create a cool dissipation effect with the right trail and get it looking fire-y, and also just be really good in general
    float4 laserColor = tex2D(laserSampler, coords);
    float threshold = coords.x;
    float d = laserColor.r < threshold;
   
    //Here we're applying a bit of extra bloom based on the y coordinate distance from the center
    laserColor.rgb *= lerp(insideColor, bloomColor, (abs(input.TextureCoordinates.y - 0.5) / 0.5));
    laserColor *= d;
    laserColor *= input.Color;
    return laserColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}