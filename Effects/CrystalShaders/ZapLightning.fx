matrix transformMatrix;
float time;
float amplitude;
float levels;
float2 tiling;
float4 bloomColor;
texture laserTexture;
sampler2D laserTex = sampler_state
{
    texture = <laserTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};

sampler distortionNoiseSampler : register(s1);
sampler gradientSampler : register(s2);

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
    float2 c2 = coords * tiling;
    float2 distortionSampleCoords = float2(time * -0.05 + c2.x, 0.0);
    float distortionNoise = tex2D(distortionNoiseSampler, distortionSampleCoords);
    float yOffset = lerp(-amplitude, amplitude, distortionNoise);
    
    //Apply the distortion
    //We'll probably use a perlin noise texture
    //Distorting the vertices creates really weird artifacts in the trail
    //So it's better to get the zigzag pattern in the pixel shader 
    float2 sampleCoords = coords;
    sampleCoords.x += time * -0.025;
    
    //By quantizing the y offset, we can make it more jagged rather than smooth
    yOffset = floor(yOffset * levels) / levels;
    sampleCoords.y += yOffset;
    float n = tex2D(laserTex, sampleCoords);
    n = pow(n, 0.5);
    
    //Apply the gradient map/palette
    float2 gradientCoords = float2(0.0, n);
    float4 gradient = tex2D(gradientSampler, gradientCoords);
    float4 finalColor = (n * gradient) * input.Color;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}