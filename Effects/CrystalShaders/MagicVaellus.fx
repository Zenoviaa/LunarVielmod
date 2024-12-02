matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;
texture outlineTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;
float power;
float alpha;

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

sampler2D outlineTex = sampler_state
{
    texture = <outlineTexture>;
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
    float4 s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    
    //Sample the main noise
    float4 sample = tex2D(primaryTex, oCoords);
    float primaryAlpha = sample.a;
    float scrollingFlamingNoise = sample.r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    
    //Sample the outline noise
    float4 outlineSample = tex2D(outlineTex, oCoords);
    float outlineAlpha = outlineSample.a;
    float osn = outlineSample.r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * primaryColor * scrollingFlamingNoise,
        noiseFlame, coords.x);
    
    //Add the outline on top
    //Calculate outline color
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, coords.x);
    
    if (outlineAlpha > 0)
    {
        return float4(outlineFlameCol, outlineAlpha) * input.Color.w;
    }
    else
    {
        return float4(flameCol, primaryAlpha) * input.Color.w;
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