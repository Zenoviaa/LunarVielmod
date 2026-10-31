#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler gradientSampler : register(s2);
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


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //first calculate some coords and oise
    float2 coords = input.TextureCoordinates.xy;
 
    float pixelated = 128.0;
 //   coords = floor(coords * pixelated) / pixelated;
    float2 noiseCoords = coords + float2(time, 0.0);
    float noise = tex2D(noiseSampler, noiseCoords * 0.2);
    float2 noiseOffset = float2(0.0, sin(noise * 3.14) * distortionStrength );
    float2 offsetCoords = coords + noiseOffset;
    float4 laserColor = tex2D(spriteSampler, offsetCoords + float2(-time, sin(-time * 12.0 + coords.x * 8.0) * 0.15));
    
    float4 outColor = float4(1.0, 0.0, 0.0, 1.0);
    outColor.rgb = ApplyHue(outColor.rgb, time * -8.0 +  coords.x * 4.0);
    if (laserColor.r > 0.5)
    {

        laserColor *= 0.0;
    }
  
    if (laserColor.r > 0.2 && laserColor.r < 0.3)
    {
   //     outColor.rgb = ApplyHue(outColor.rgb, time * 16.0 + coords.y * 4.0 + coords.x * 4.0);
        laserColor *= 3.0;
        laserColor += 5.5;
    }
    else
    {
        laserColor += tex2D(spriteSampler, offsetCoords + float2(-time, -time * 0.2));
        //outColor.rgb = float3(1.0, 1.0, 1.0);
        //laserColor *= 3.0;
        //laserColor += 5.5;
    }
    laserColor += QuadraticBump(coords.y) * 0.2;
    laserColor *= input.Color;
    laserColor *= outColor;
    laserColor *= lerp(1.0, 3.0, coords.x);

    laserColor = floor(laserColor * 8.0) / 8.0;
    laserColor *= 1.0 - coords.x;
    laserColor *= QuadraticBump(coords.y);
    laserColor.a = 0.0;
    return laserColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}