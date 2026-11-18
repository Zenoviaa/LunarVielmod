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

Texture3D ColorSpectrumTexture;
sampler3D ColorSpectrumTextureSampler = sampler_state
{
    Texture = <ColorSpectrumTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

Texture2D EdgeTexture;
sampler2D EdgeTextureSampler = sampler_state
{
    Texture = <EdgeTexture>;
    AddressU = clamp;
    AddressV = clamp;
};


Texture2D brightenNoiseTexture;
sampler2D brightenNoiseSampler = sampler_state
{
    Texture = <brightenNoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

Texture2D causticsNoiseTexture;
sampler2D causticsNoiseSampler = sampler_state
{
    Texture = <causticsNoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};
Texture2D foamNoiseTexture;
sampler2D foamNoiseSampler = sampler_state
{
    Texture = <foamNoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};


float time;
float levels;
float distortion;
float3 startGradient;
float3 endGradient;
float4 causticsColor;
float2 tiling;
float2 screenOffset;
float foamLava;

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float4 SampleSpriteNoise(in VertexShaderOutput input, sampler2D SpriteSampler)
{
    float2 coords = input.TextureCoordinates;
        
    //Distort the coordinates
    float d = tex2D(SpriteTextureSampler, coords);
    float2 distortionOffset = float2(sin(d), cos(d)) * distortion;
 
    coords += distortionOffset;
    
    float2 offset = float2(time * -0.05, 0.0);
    float2 offset2 = float2(time * 0.05, 0.1);
    
    float4 sample1 = tex2D(SpriteTextureSampler, coords + offset);
    float4 sample2 = tex2D(SpriteTextureSampler, coords + offset2);
    
    
    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color * input.Color;
    
    finalColor.r = posterize(finalColor.r, levels);
    finalColor.g = posterize(finalColor.g, levels);
    finalColor.b = posterize(finalColor.b, levels);
    return finalColor;
}

float4 SampleCausticsNoise(in VertexShaderOutput input, sampler2D SpriteSampler)
{
    float2 coords = input.TextureCoordinates;

    
    float t = time;
    t *= 2.0;
    
    float d = tex2D(SpriteSampler, coords + float2(t * -0.02, t * -0.04)).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    float2 offset = float2(t * -0.05, 0.0);
    float2 offset2 = float2(t * 0.05, 0.3);
    
    float4 sample1 = tex2D(SpriteSampler, distortedCoords + offset);
    float4 sample2 = tex2D(SpriteSampler, distortedCoords + offset2);
    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color;
    return finalColor;
}

float4 SampleFoam(float2 coords)
{
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float2 offsetCoords = (coords * tiling * 2.0) + float2(0.0, time * -0.05);
    offsetCoords += screenOffset * 4.0;
    
    float foam = tex2D(foamNoiseSampler, offsetCoords);
    float power = lerp(8.0, 0.3, heightMapColor.a);
    foam = pow(foam, power);
    float4 foamColor = float4(foam, foam, foam, 1.0);
    return foamColor * foam * 2.0;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    input.TextureCoordinates *= tiling;
    input.TextureCoordinates += screenOffset * tiling;
    input.TextureCoordinates = frac(input.TextureCoordinates);
    float4 baseColor = SampleSpriteNoise(input, SpriteTextureSampler) + SampleSpriteNoise(input, brightenNoiseSampler) * 0.5;
    
    float4 gradientColor = baseColor;
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float3 gradient = lerp(endGradient, startGradient, heightMapColor.a);
    gradient *= gradient;
    gradientColor.rgb *= gradient;
    
    float4 caustics = SampleCausticsNoise(input, causticsNoiseSampler);
    
    float4 foam = SampleFoam(coords);
    foam *= foamLava;
    return baseColor + caustics * causticsColor ;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
