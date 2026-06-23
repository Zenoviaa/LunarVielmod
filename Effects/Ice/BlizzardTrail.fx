sampler windSampler : register(s0);
sampler snowSampler : register(s1);
matrix transformMatrix;
float2 windImageSize;
float2 snowTexelSize;
float3 insideColor;
float3 bloomColor;
float time;


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
    float2 coords = input.TextureCoordinates;
    coords.y += sin(time * -0.3 + coords.x * 14.0) * 0.1;
    
    float2 windCoords = coords + float2(time * -0.05, 0.0);
    float ySin = sin(coords.x * 8.0 + time * -0.15) * 0.5;
    windCoords.y += ySin;
    windCoords = frac(windCoords);
    float4 windColor = tex2D(windSampler, frac(windCoords * 0.3));

    
    float2 snowCoords = coords * windImageSize * snowTexelSize;
    snowCoords += float2(time * -0.05, 0.0);
    snowCoords.y += ySin * 0.7;
    snowCoords = frac(snowCoords * 0.9);
    float4 snowColor = tex2D(snowSampler, snowCoords);
   
    snowColor *= 0.4;
    
    float ySinFade = sin(coords.y * 3.14) ;
    snowColor *= ySinFade * 2.5;
    float d = snowColor.r > 0.4;
    float d2 = snowColor.r > coords.x;
    snowColor *= d;
    snowColor *= 0.5;
    
    
    float yFade = abs(coords.y - 0.5) / 0.5;
    float3 bloomingColor = lerp(insideColor, bloomColor, pow(yFade, 2.0));
    float4 blizzardingMix = windColor + snowColor;
    blizzardingMix.rgb *= bloomingColor;
 
    float osc2 = sin( time + coords.x * 8.0);
    float3 extraBloom = bloomingColor * ySinFade * 0.4;
   
    blizzardingMix.rgb += pow((1.0 - coords.x), 8.0) * ySinFade;
    blizzardingMix.rgb += extraBloom;
    blizzardingMix *= input.Color;
    blizzardingMix *= d2;
    
    return blizzardingMix;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}