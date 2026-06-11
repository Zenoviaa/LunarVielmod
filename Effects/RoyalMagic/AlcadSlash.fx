sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
matrix transformMatrix;
float time;
float distortion;
float3 bloomColor;


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


float2 SampleDistortionOffset(in float2 coords)
{
    float2 noiseCoords = coords + float2(time * -0.05, time * 0.025);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(uImage1, noiseCoords).r;

    float2 offset = float2(0.0, 1.0 * sin(n));
    float2 distortionOffset = offset * distortion;
    return distortionOffset;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    float4 sampleColor = input.Color;
    float4 maskColor = tex2D(uImage0, coords + SampleDistortionOffset(coords));
    float4 scrollingColor = tex2D(uImage2, frac(coords + float2(time * -0.05, 0.0)));
    scrollingColor.rgb *= lerp(bloomColor.rgb, float3(1.0, 1.0, 1.0), saturate(sin(coords.y * 3.14)));
    float4 finalColor = maskColor + (scrollingColor * maskColor.r) ;
    finalColor *= sampleColor;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}