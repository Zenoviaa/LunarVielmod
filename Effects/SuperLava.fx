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
sampler NormalNoiseSampler : register(s1);
sampler HeightMapSampler : register(s2);
sampler GlowSampler : register(s3);
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float reflectionDistance;
float2 reflectionTexelSize;
float reflectionPower;
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
    float sample3 = tex2D(SpriteTextureSampler, frac((coords + offset3) * 2.5)).r;
    float sample4 = tex2D(GlowSampler, frac((coords + offset4) * 1.5)).r;
    
    
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
    float4 gradientColor = baseColor;
    float4 heightMapColor = tex2D(HeightMapSampler, coords);
    float3 gradient = lerp(EndGradient, StartGradient, heightMapColor.a);
    float4 rocksColor = SampleRocks(input, RockTextureSampler);
    float rockAlpha = 1.0 - heightMapColor.a;
    rocksColor *= rockAlpha;
    rocksColor.rgb = lerp(rocksColor.rgb, gradientColor.rgb, 0.5);
    float4 finalColor = gradientColor  + float4(gradient, 1.0);
    return finalColor;
}


float4 ReflectPS(VertexShaderOutput input) : COLOR
{
    //This will flip the sprite within itself
    //Here's how I think it should have to work
    
    //Step 1. Sample the current height map at this point
    float2 coords = input.TextureCoordinates;
    float heightSample = tex2D(HeightMapSampler, coords).a;
    
    //Cubing the height gradient so it's not so long
    //For some reason this is faster than using pow
    float heightGradient = pow(heightSample, reflectionPower);
    
    //Step 2. Using the height map gradient, calculate how far upwards we should get the pixel
    //A value of 1 means were at the surface, so we shouldn't look that far upward
    //A value of 0 means we're at the bottom so we should go very far upward
    //We'll control this with a parameter
    float reflectionFactor = smoothstep(1.0, 0.1, heightGradient);
    float2 reflectionOffset = float2(0.0, -reflectionDistance * reflectionFactor) * reflectionTexelSize;
    
    //Step 3. Sample the new coordinates, that's our pixel
    //With how this works, it should also flip the sprite I think
    float2 reflectedCoords = coords + reflectionOffset;
    
    //Step 4. Distortion
    float d = tex2D(NoiseTextureSampler, coords + float2(Time * -0.02, Time * -0.04) + ScreenOffset).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), 0.0) * Distortion;
 
    reflectedCoords += distortionOffset;
    float4 color = tex2D(ClampTextureSampler, reflectedCoords);
    
    //Step 4. blend the reflection with the height gradient so there's no reflection deep in the water
    float4 fadedColor = color * heightGradient * heightGradient;
    float4 finalColor = fadedColor * input.Color;
    return finalColor;
}


float4 CombineALLPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    
    
    float4 baseWaterColor = tex2D(SpriteTextureSampler, coords);
            //sample outline
    if (baseWaterColor.a <= 0.0)
    {
        float2 leftCoords = coords + float2(-outlineTexelSize.x, 0.0);
        float2 rightCoords = coords + float2(outlineTexelSize.x, 0.0);
        float2 upCoords = coords + float2(0.0, -outlineTexelSize.y);
        float2 downCoords = coords + float2(0.0, outlineTexelSize.y);
   
        float4 left = tex2D(SpriteTextureSampler, leftCoords);
        float4 right = tex2D(SpriteTextureSampler, rightCoords);
        float4 up = tex2D(SpriteTextureSampler, upCoords);
        float4 down = tex2D(SpriteTextureSampler, downCoords);
    
        
        if (left.a > 0)
            return left.a * outlineColor;
        if (right.a > 0)
            return right.a * outlineColor;
        if (up.a > 0)
            return up.a * outlineColor;
        return down.a * outlineColor;
    }

    
    float4 fancyWaterColor = tex2D(NormalNoiseSampler, coords);
    fancyWaterColor.r = posterize(fancyWaterColor.r, Quantize);
    fancyWaterColor.g = posterize(fancyWaterColor.g, Quantize);
    fancyWaterColor.b = posterize(fancyWaterColor.b, Quantize);
    
    float4 finalColor = fancyWaterColor * baseWaterColor.a;
    return finalColor * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
technique ReflectionDrawing
{
    pass P3
    {
        PixelShader = compile PS_SHADERMODEL ReflectPS();
    }
};
technique Combine
{
    pass P11
    {
        PixelShader = compile PS_SHADERMODEL CombineALLPS();
    }
};
