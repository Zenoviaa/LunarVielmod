

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
#define PI 3.14159

//Vars
Texture2D SpriteTexture;
SamplerState TextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

Texture2D NoiseTexture;
SamplerState NoiseTextureSampler = sampler_state
{
    Texture = <NoiseTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

float Interpolant;
float Distortion = 0.2;
float2 Tiling;
float3 InnerColor;
float3 GlowColor;
float3 UnderToneColor;
float InExpo(float t)
{
    const float p = 5.0;
    return pow(2, p * t - p);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    //Convert to polar coordinates and scroll outward
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;

    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    

    float2 polarUV = float2(angle, dist);
    float2 polarOffset = lerp(float2(0.0, 0.0), float2(-1.0, -1.0), Interpolant);
    float2 noisePolarUV = polarUV + polarOffset;
    
    //Sample the noise and use it to distort the main texture
    //Which is just a circle
    float noise = NoiseTexture.Sample(NoiseTextureSampler, noisePolarUV * Tiling);
    
    //noise is a lerp value for how much to distort by
    
    float2 offset = lerp(float2(0.0, 0.0), polarUV, noise);
    offset *= Distortion;
    float2 c = noisePolarUV + offset;
    
    //Convert to a vector ot make it pulse outward
    float4 color = SpriteTexture.Sample(TextureSampler, c) * sampleColor;
   
    //Calculate main color
    float3 color2 = lerp(InnerColor, GlowColor, dist);
    color.rgb *= color2;
    
    //Calculate Undertone
    float4 underTone = SpriteTexture.Sample(TextureSampler, c - offset * 0.5) * sampleColor;
    underTone.rgb *= UnderToneColor;
      
    //Fade out the edges
    float4 final = underTone + color;
    float fade = 1.0 - dist;
    return final * fade;
}

technique SpriteDrawing
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};