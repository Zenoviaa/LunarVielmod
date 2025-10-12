matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;

float time;
float distortion;
float2 tiling;
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

float2 DistortCoordinates(float2 coords)
{
    float2 offsetCoords = coords + float2(time * -0.025, 0.0);
    float sample = tex2D(noiseTex, offsetCoords * tiling);
    
    //This will make it so the back of the trail is heavily distorted and the front isn't
    float fade = smoothstep(0.0, 1.0, coords.x);
    return coords + float2(0.0, sin(sample) * distortion * fade);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 coords = input.TextureCoordinates;
    float2 distortedCoords = DistortCoordinates(coords);

    //Glow from middle outward, that's why we're using y here
    float interpolant = QuadraticBump(distortedCoords.y);
    float4 color = tex2D(primaryTex, distortedCoords + float2(time * -0.05, 0.0));
    color = pow(color, power);
    color.rgb *= lerp(innerColor, outerColor, interpolant);
    return color * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}