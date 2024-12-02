matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;

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
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Calculate Distortion
    float2 coords = input.TextureCoordinates;
    float distortionNoise = tex2D(primaryTex, coords + float2(time * -0.05, 0.0)).r;
    
    //Scrolling Noise
    float rot = distortionNoise * 3.142;
    float2 scrollingNoise = coords + float2(sin(rot) * distortion, time * 0.3);
    
    //Scroll multiple directions and lerp together to make a cool effect
    float3 col = tex2D(primaryTex, coords + scrollingNoise).rgb * 0.5;
    for (float f = 0.0; f < 1.0; f += 0.25)
    {
        float rot = f * 3.14;
        float speed = 0.2;
        float2 offsetCoords = coords + float2(
            sin(rot) * time * speed,
            cos(rot) * time * speed);
                           
        float colorMap = tex2D(noiseTex, offsetCoords + scrollingNoise).r;
        float3 targetCol = lerp(primaryColor, noiseColor, colorMap);
        float4 s = tex2D(primaryTex, offsetCoords + scrollingNoise);
        col = lerp(col, targetCol, 0.25);
    }
  
    //Outline it
    float2 diff = coords - float2(0.5, 0.5);
    float l = pow(length(diff), 2.5);
    float factor = l / 0.5;
    float3 finalCol = lerp(col, outlineColor, factor);
    float4 c = float4(finalCol, input.Color.w);
    float4 t = lerp(c, c * 0.0, factor) * (1.0 - factor);
    return t;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}