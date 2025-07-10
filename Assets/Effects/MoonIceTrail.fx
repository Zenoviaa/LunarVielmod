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

texture noiseTexture;
sampler2D NoiseSampler = sampler_state
{
    texture = <noiseTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};

float Time;
float2 Offset;
float2 Tiling;
float Distortion;
float4 BaseColor;
float4 HighlightColor;
float4 InnerGlowColor;
float4 InnerCoreColor;
float4 SparkleColor;
#define PI 3.14159

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

float QuadraticBump(float time)
{
    return time * (4.0 - time * 4.0);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 noiseUv = input.TextureCoordinates;
    noiseUv *= Tiling;
    noiseUv += float2(Time * -0.05, 0.0);
    
    //Scroll a noise and use it to distort the main coordinates
    float noiseSample = tex2D(NoiseSampler, noiseUv);
    float rot = lerp(0, 3.14, noiseSample);
    float2 angleOffset = float2(sin(rot), cos(rot));
    float2 uv = input.TextureCoordinates;
    uv += angleOffset * Distortion;
    uv += Offset;
    
    //Wanna draw a darker space-y trail in the inside, so where the y value is between 0.2 and 0.8 probably?
    //Alright, let's do some more sampling
    float4 trailSample = tex2D(trailTex, uv);
    float4 trailColor = trailSample * lerp(BaseColor, HighlightColor, noiseSample);
    
    //Calculate inner glow
    float yInterpolant = QuadraticBump(input.TextureCoordinates.y);
    float4 innerColor = lerp(InnerCoreColor, InnerGlowColor, yInterpolant);
    
    float4 mainColor = trailColor;
    float y = input.TextureCoordinates.y;
    
    float radiusOff = (noiseSample * 0.01f) + 0.05;
    if (y > 0.5 - radiusOff && y <= 0.5 + radiusOff)
    {
        if(noiseSample >= 0.6)
            return SparkleColor;
        return innerColor * input.Color;
    }
    else
    {
        return mainColor * input.Color;
    }
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}