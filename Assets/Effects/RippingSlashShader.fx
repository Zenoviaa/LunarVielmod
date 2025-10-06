
//Vars
Texture2D SpriteTexture;
SamplerState TextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

matrix World;
matrix TransformMatrix;
Texture2D NoiseTexture;
SamplerState NoiseTextureSampler = sampler_state
{
    Texture = <NoiseTexture>;
};

float2 NoiseTiling;
float2 NoiseOffset;
float Distortion;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Noise factor
    float2 noiseCoords = (coords * NoiseTiling) + NoiseOffset;
    float noise = NoiseTexture.Sample(NoiseTextureSampler, noiseCoords);
    
    //Calculate distortion
    float2 offset = float2(cos(noise), sin(noise)) * Distortion;
    float4 color = SpriteTexture.Sample(TextureSampler, coords + offset);
    return color * sampleColor;
}

technique SpriteDrawing
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};