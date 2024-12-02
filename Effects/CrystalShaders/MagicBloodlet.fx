matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;
float alpha;

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
    // Normalized pixel coordinates (from 0 to 1)
    //Calculate Distortion
    float2 coords = input.TextureCoordinates;
    float d = tex2D(noiseTex, coords + float2(time * -0.1, time * -0.05)).r;
    float offset = d * distortion;
    float distortionNoise = tex2D(primaryTex, coords + offset + float2(time * -0.05, 0.0)).r;
    float distortion = 0.1;
    
    //Scrolling Noise
    float rot = distortionNoise * 4.142;
    float2 scrollingNoise = coords + float2(sin(rot) * distortion, time * 0.3);
    
    //Scroll multiple directions and lerp together to make a cool effect
    float3 col = tex2D(noiseTex, coords + scrollingNoise).rgb * 0.5;
    for (float f = 0.0; f < 1.0; f += 0.25)
    {
        float rot = f * 3.14;
        float speed = 0.2;
        float2 offsetCoords = coords + float2(
            sin(rot) * time * speed,
            cos(rot) * time * speed);
                           
        float colorMap = tex2D(noiseTex, offsetCoords + scrollingNoise).r;
        float3 targetCol = lerp(primaryColor, noiseColor, colorMap);
        col = lerp(col, targetCol, 0.25);
    }
  

    //Outline it
    float3 outlineCol = float3(0.0, 0.0, 0.0);
    float2 diff = coords - float2(0.5, 0.5);
    float l = pow(length(diff), 2.5);
    float factor = l / 0.5;
    float3 finalCol = lerp(col, outlineCol, factor);
    
    float3 trailCol = d * finalCol;
    float p = tex2D(primaryTex, coords + float2(time * -0.05, 0.0)).r;
    p *= d;
    
    if (p > alpha)
    {
        return float4(trailCol, 1.0);

    }
    else
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}