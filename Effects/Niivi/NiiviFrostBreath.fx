#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler noiseSampler2 : register(s2);
sampler ditherSampler : register(s3);
float2 spriteSize;
float2 noiseTexelSize;
float2 noise2TexelSize;
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


float4 SampleNoise(float2 coords)
{
    float2 scrollingCoords = coords - float2(time, 0.0);
    float4 noiseColor = tex2D(noiseSampler, scrollingCoords * 1.5);
    return noiseColor * 1.1;
}
float4 SampleNoiseLight(float2 coords)
{
    float2 scrollingCoords = coords - float2(time + 0.3, 0.0);
    float4 noiseColor = tex2D(noiseSampler, scrollingCoords * 1.5);
    return noiseColor * 7.0;
}

float4 SampleCrystals(float2 coords)
{
    float t = time * 0.7;
    //t = floor(t * 16.0) / 16.0;
    
    coords.y += sin(coords.x * 4.0 + t * 8.0) * 0.1;
    float2 scrollingCoords = coords - float2(t, 0.0);
    float4 noiseColor = tex2D(noiseSampler2, scrollingCoords );
    float4 redColor = float4(noiseColor.r, 0.0, 0.0, 1.0);
//redColor.rgb = ApplyHue(redColor.rgb, noiseColor.r * 6.0 + t * 25.0 + coords.x );
    noiseColor *= QuadraticBump(coords.y);
    float d = coords.x;
    float m = noiseColor.r > d; //noiseColor *= redColor;
    return noiseColor * m * (sin(time * -12.0 + coords.x * 12.0) * 0.5 + 0.95);
}
float4 SamplePrismaticWaves(float2 coords)
{
    float a = QuadraticBump(coords.y);
    //float a = tex2D(noiseSampler, coords + float2(time * -1.0, 0.0));
    coords *= 2.0;
    coords.x -= time;
    coords.x += sin(coords.y * 12.0) * 0.03;
    float4 redColor = float4(1.0, 0.0, 0.0, 1.0);
    redColor.rgb = ApplyHue(redColor.rgb, coords.x * 52.0 + time * 2);
    redColor *= sin(coords.x * 24.0);
    redColor *= a;
    
  //  redColor *= a;
    return redColor * 0.3;
}

float4 SampleDithering(float2 coords)
{
    float2 ditherCoords = coords * spriteSize * noiseTexelSize;
    float4 ditherColor = tex2D(ditherSampler, coords);
    return ditherColor;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //So frost breath that zemmie wants seems to be defined by a little laser in the middle similar to rek
    //With a faint smoky outside that's heavily distorting, more the further out it goes
    //and the nseveral little smoke particles, icey bits, and snowflakes on top
    //It should also curve a little bit and spark near the edges
    //whre it impacts the ground it should create erupting icicles
    
    //first lets get the laser rendering then cook this shader
    float2 coords = input.TextureCoordinates.xy;
    //coords = floor(coords * 128.0) / 128.0;
    float4 noiseColor = SampleNoise(coords);
    float4 noiseColorLight = SampleNoiseLight(coords);
    float4 prismaticWaves = SamplePrismaticWaves(coords);
    float4 crystals = SampleCrystals(coords);
    float4 ditherColor = SampleDithering(coords);
//    return crystals;
    //coords *= 0.7;
    coords.y += sin(time * 4.0 + coords.x * 3.0) * 0.1;
    float b = coords.y * (1.0 - coords.y) * 2.0;
    float2 scrollingCoords = coords + float2(-time * 1.7, 0.0);
    float4 spriteColor = tex2D(spriteSampler, scrollingCoords);
    spriteColor *= coords.x;
    spriteColor *= 3.0;
    
    float4 bloomColor = float4(0.3, 0.86, 0.85, 1.0);
    bloomColor *= b;
    bloomColor *= pow(coords.x, 0.25);
    
    float4 whiteBloomColor = float4(1.0, 0.0, 1.0, 1.0);
    whiteBloomColor *= b;
    whiteBloomColor *= pow(coords.x, 0.25);
    
    spriteColor += bloomColor;
    spriteColor += whiteBloomColor * 0.3;
    spriteColor *= noiseColor;
    spriteColor += noiseColorLight * spriteColor.r;
    spriteColor -= ditherColor * 0.3;
    spriteColor = floor(spriteColor * 3.0) / 3.0;
    spriteColor.rgb = ApplyHue(spriteColor.rgb, coords.x * 28.0 + time * -4.0);
    spriteColor += bloomColor * 2.7;
    spriteColor += prismaticWaves;
 //   spriteColor -= crystals * 2.0;
    spriteColor += QuadraticBump(coords.y) * 0.16;
    spriteColor += crystals * 1.0;
    

    return spriteColor * 0.6 * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}