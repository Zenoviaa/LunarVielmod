#define PS_SHADERMODEL ps_3_0

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D ClampTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = clamp;
    AddressV = clamp;
};

Texture2D HeightMapTexture;
sampler2D HeightMapTextureSampler = sampler_state
{
    Texture = <HeightMapTexture>;
};


Texture2D WaterTexture;
sampler2D WaterTextureSampler = sampler_state
{
    Texture = <WaterTexture>;
    AddressU = clamp;
    AddressV = clamp;
};


Texture2D NoiseTexture;
sampler2D NoiseTextureSampler = sampler_state
{
    Texture = <NoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};
Texture2D RockTexture;
sampler2D RockTextureSampler = sampler_state
{
    Texture = <RockTexture>;
    AddressU = wrap;
    AddressV = wrap;
};
float3 InnerColor;
float3 BloomColor;

float3 StartGradient;
float3 EndGradient;


float2 ScreenOffset;
float2 Tiling;
float Time;
float Quantize;
float Distortion;
float NormalDistortionStrength;

Texture2D NormalNoiseTexture;
sampler2D NormalNoiseSampler = sampler_state
{
    Texture = <NormalNoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};


sampler GlowSampler : register(s3);
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 outlineColor;
float2 outlineTexelSize;
float posterize(float v, float k)
{
    return ceil(v * k) / k;
}

float4 SampleSpriteNoise(in VertexShaderOutput input, sampler2D SpriteSampler)
{
    float2 coords = input.TextureCoordinates;
        
 
    //Here we'll distort the texture with a scrolling normal noise texture, this should create cool and interesting movements
    float3 normalVec = tex2D(NormalNoiseSampler, coords).rgb;
    normalVec *= 2.0;
    normalVec -= 1.0;
    
    float2 normalOffset = normalVec.xy;
    float2 distortionOffset = normalOffset * NormalDistortionStrength;
    coords *= Tiling;
    coords += distortionOffset;

    float2 offset = float2(Time * -0.05, 0.0);
    float2 offset2 = float2(Time * 0.05, 0.2);
    float2 offset3 = float2(Time * -0.05, 0.6);
    float2 offset4 = float2(Time * 0.025, 0.44);
    
    float sample1 = tex2D(SpriteTextureSampler, frac(coords + offset)).r;
    float sample2 = tex2D(SpriteTextureSampler, frac(coords + offset2)).r;
    float sample3 = tex2D(SpriteTextureSampler, frac((coords + offset3) )).r;
    float sample4 = tex2D(RockTextureSampler, frac((coords + offset4) )).r;
    
    
 //   float combinedSample = (sample1 + sample2) / 2.0;
    float3 color1 = lerp(InnerColor, BloomColor, sample1);
    float3 color2 = lerp(InnerColor, BloomColor, sample2);
    
    float3 color = color1 + color2;
    color /= 2.0;

    float4 finalColor = float4(color, 1.0) * input.Color;
    finalColor -= sample3 * 0.8;
    
    float osc = sin(sample4 * 6.28 + Time * -0.05) * 0.5 + 0.5;
    finalColor += lerp(-0.3, 0.6, osc) * 0.7;

    return finalColor;
}


float4 SampleRocks(in VertexShaderOutput input, sampler2D SpriteSampler)
{
    float2 coords = input.TextureCoordinates;
        
    //Here we'll distort the texture with a scrolling normal noise texture, this should create cool and interesting movements
    float3 normalVec = tex2D(NormalNoiseSampler, coords).rgb;
    normalVec *= 2.0;
    normalVec -= 1.0;
    
    float2 normalOffset = normalVec.xy;
    float2 distortionOffset = normalOffset * NormalDistortionStrength;
    coords *= Tiling * 2.0 * float2(0.5, 1.0);
    coords += distortionOffset;

    float2 offset = float2(Time * -0.015, 0.0);
    float4 sample1 = tex2D(RockTextureSampler, frac(coords + offset));
    return sample1;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    input.TextureCoordinates += ScreenOffset;
    input.TextureCoordinates = frac(input.TextureCoordinates);
    
    float4 baseColor = SampleSpriteNoise(input, SpriteTextureSampler);
    float4 finalColor = baseColor;
    return finalColor * 1.4;
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
