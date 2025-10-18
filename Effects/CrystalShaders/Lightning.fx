matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;

float time;
float power;
float3 innerColor;
float3 outerColor;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
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

float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float interpolant = QuadraticBump(coords.y);
    
    //Powing it should make the out color more prominent
    //Sining so we can make like a flash effect in projectiles

    float3 lightningColor = lerp(outerColor, innerColor, interpolant);
    float4 sampleColor = tex2D(primaryTex, coords);
    sampleColor = pow(sampleColor, power);
    
    //Sample color
    float4 finalColor = sampleColor;
    finalColor.rgb *= lightningColor;
    finalColor *= interpolant;
    
    
    float4 sampleNoise = tex2D(noiseTex, coords + float2(time * 0.05, 0.0));
    sampleNoise.rgb *= outerColor;
    finalColor += sampleNoise * 0.2f;
    
    return finalColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}