sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
matrix uWorldViewProjection;
float4 uShaderSpecificData;

texture primaryTexture;
texture noiseTexture;

float3 primaryColor;
float3 noiseColor;

float time;
float distortion;
float power;

sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


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
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 coords = input.TextureCoordinates;
    
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(primaryTex, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
 
    //Scrolling noise color
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * primaryColor * scrollingFlamingNoise,
        noiseFlame, coords.x);

    float3 finalCol = flameCol;
    
    // Output to screen
    float4 finalColor = float4(finalCol, 1.0) * input.Color.w;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}