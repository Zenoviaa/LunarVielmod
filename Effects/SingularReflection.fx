#define PS_SHADERMODEL ps_3_0

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

Texture2D HeightMapTexture;
sampler2D HeightMapTextureSampler = sampler_state
{
    Texture = <HeightMapTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct HeightPixelShaderOutput
{
    float4 Height : SV_Target0;
    float4 Light : SV_Target1;
};

float reflectionDistance;
float2 reflectionTexelSize;
float reflectionPower;

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    //This will flip the sprite within itself
    //Here's how I think it should have to work
    
    //Step 1. Sample the current height map at this point
    float2 coords = input.TextureCoordinates;
    float heightSample = tex2D(HeightMapTextureSampler, coords).r;
    
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
    float4 color = tex2D(SpriteTextureSampler, reflectedCoords);
    
    //Step 4. blend the reflection with the height gradient so there's no reflection deep in the water
    float4 fadedColor = color * heightGradient * heightGradient;
    float4 finalColor = fadedColor * input.Color;
    return finalColor;
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
