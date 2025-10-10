matrix transformMatrix;
texture innerTexture;
texture trailTexture;
texture noiseTexture;
texture trailOutlineTexture;

float3 outlineColor;
float time;
float distortion;
float power;
float threshold;
float tightness;

sampler2D innerTex = sampler_state
{
    texture = <innerTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D trailTex = sampler_state
{
    texture = <trailTexture>;
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

sampler2D trailOutlineTex = sampler_state
{
    texture = <trailOutlineTexture>;
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
    // Normalized pixel coordinates (from 0 to 1)
    float2 coords = input.TextureCoordinates;
    
    
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    //Calculate the trail
    float4 trailSample = tex2D(trailTex, oCoords);
    float4 trailOutlineSample = tex2D(trailOutlineTex, oCoords);
    
    //Scroll the noise
    float scrollingTrail = trailSample.r;
    scrollingTrail = pow(scrollingTrail, power);
    
    float2 c = coords + float2(time * -0.25, 0.0);
    c *= tightness;
    float4 innerTrailSample = tex2D(innerTex, c);
    
    if (trailOutlineSample.a > 0)
    {
        return float4(outlineColor * trailOutlineSample.rgb, trailOutlineSample.a);
    }
    else if (trailSample.a > 0)
    {
        return float4(innerTrailSample.rgb, trailSample.a);
    }
    else
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}