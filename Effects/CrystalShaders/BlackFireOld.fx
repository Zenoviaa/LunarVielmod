matrix transformMatrix;



float time;
float2 tiling;
float3 innerColor;
float3 outerColor;
float3 backColor;
texture primaryTexture;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};


texture primaryTexture2;
sampler2D primaryTex2 = sampler_state
{
    texture = <primaryTexture2>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};


texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float distortion;
texture distortionTexture;
sampler2D distortionTex = sampler_state
{
    texture = <distortionTexture>;
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

float2 DistortCoordinates(float2 coords)
{
    float n = tex2D(distortionTex, coords + float2(time * -0.1, 0.0));
    float2 distortedCoords = coords;
    distortedCoords.y += lerp(-1.0, 1.0, n) * distortion;
    distortedCoords.y = saturate(distortedCoords.y);
    return distortedCoords;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float2 distortedCoords = DistortCoordinates(coords);
    
    float n1 = tex2D(noiseTex, (distortedCoords + float2(time * -0.05, 0.0)) * tiling);
    float n2 = tex2D(noiseTex, (distortedCoords + float2(time * -0.08, 0.0)) * tiling);
    float noise = saturate(n1 + n2);
    noise = pow(noise, 0.5);
    
    float fade = smoothstep(0.0, 1.0, coords.x);

    float3 fireColor = lerp(outerColor, innerColor, noise);
    fireColor = lerp(fireColor, backColor, fade);
    
    float n3 = tex2D(noiseTex, (distortedCoords + float2(time * -0.10, 0.0)) * tiling);
    float3 flareColor = lerp(outerColor, innerColor, pow(n3, 0.5));
    
    float mask = tex2D(primaryTex, distortedCoords + float2(time * -0.025, 0.0));
    float4 trailColor1 = float4(saturate(fireColor + flareColor), mask) * input.Color;
    
    float mask2 = tex2D(primaryTex2, distortedCoords + float2(time * -0.025, 0.0));
    float4 trailColor2 = float4(lerp(outerColor, innerColor, mask2), mask2) * input.Color;
    return saturate(trailColor1 + trailColor2);
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}