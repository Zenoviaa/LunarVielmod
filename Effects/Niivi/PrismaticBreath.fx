#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float distortionStrength;

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

float4 SampleNoiseColor(float2 coords)
{
    float speed = 1.0;
    float2 noiseCoords = coords * 2.0;
    noiseCoords.y *= 0.125;

    float t = time * speed;
    float2 scrollingCoords = noiseCoords + float2(-t, 0.0);
    //scrollingCoords.y += t * 8.0;
    float4 noiseColor1 = tex2D(noiseSampler, scrollingCoords);
    noiseColor1 = floor(noiseColor1 * 4.0) / 4.0;
    return noiseColor1;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //first calculate some coords and oise
    float2 coords = input.TextureCoordinates.xy;
    float4 noiseColor = SampleNoiseColor(coords);
    float2 scrollingCOords = coords;
    scrollingCOords.x += sin(noiseColor.r * 3.16);
    scrollingCOords.x -= time;
    float4 spriteColor = tex2D(spriteSampler, scrollingCOords);
    float4 finalColor = spriteColor * input.Color;

    
    float4 bloomColor = float4(1.0, 0.0, 0.0, 1.0);
    bloomColor *= QuadraticBump(coords.y) * 0.8;
    bloomColor.rgb = ApplyHue(bloomColor.rgb, time * -12.0 + scrollingCOords.x * 4.0);
    
    float4 bloomColor2 = QuadraticBump(coords.y) * 0.5;
    bloomColor2 = pow(bloomColor2, 3.0);
    finalColor += bloomColor2;
    finalColor += bloomColor;
//    finalColor += (sin(time * 16.0) * 0.5 + 0.5) * finalColor.r;
    finalColor *= noiseColor;
 //   finalColor.rgb = ApplyHue(finalColor.rgb, time + scrollingCOords.x * 4.0);
    //finalColor *= sin(scrollingCOords.x * 4.0) * 0.5 + 0.5;
    finalColor *= 2.0;
   // finalColor *= (1.0 - coords.x);
    finalColor.a = 0.0;
    return finalColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}