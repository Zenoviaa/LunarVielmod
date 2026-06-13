sampler glowMaskSampler : register(s0);
sampler noiseSampler : register(s1);
matrix transformMatrix;
float3 flameStartColor;
float3 flameBloomColor;
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
    float4 tintColor = input.Color;
    float2 coords = input.TextureCoordinates;
    //Apply distortion to the blowtorch so it doesn't just look like one straight circle lol
    float2 noiseCoords = coords + float2(time * 0.05, 0.0);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(noiseSampler, noiseCoords).r;
    float2 distortionOffset = float2(0.0, sin(n * 3.14 * 8.0)) * 0.01;
    float2 sampleCoords = coords + distortionOffset;
    
    //Applying the same technique again, we scroll a 
    //This time we're going to actually apply a gradient to the glow so it isn't the asme glow from the start to the end
    //This will genuinely just look better
    //I think we go from yellow to red
    float4 glowMask = tex2D(glowMaskSampler, sampleCoords) * tintColor;
    float3 flameColor = lerp(flameStartColor, flameBloomColor, coords.x);
    glowMask.rgb *= flameColor;
    glowMask.rgb += lerp(1.5, 0.0, saturate(coords.x + time)) * glowMask.r * 8.0;
    return glowMask;
}

technique Technique1
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}