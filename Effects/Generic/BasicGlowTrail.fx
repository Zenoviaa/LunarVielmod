matrix transformMatrix;
float4 insideColor;
float4 glowColor;
float4 bloomColor;

//https://stackoverflow.com/questions/28900598/how-to-combine-two-colors-with-varying-alpha-values
float4 AlphaBlend(float4 fg, float4 bg)
{
    float4 result;
    float a = fg.a;
    float r = (fg.r * a + bg.r * (1 - a));
    float g = (fg.g * a + bg.g * (1 - a));
    float b = (fg.b * a + bg.b * (1 - a));
  
    
    result.r = r;
    result.g = g;
    result.b = b;
    result.a = max(fg.a, bg.a);
    return result;
}

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

float4 GlowVersion1(VertexShaderOutput input)
{
    float2 coords = input.TextureCoordinates;
    float interp = (sin(coords.y * 3.14) * 0.5 + 0.5);
    float4 backGlow = bloomColor * interp;
    
    float innerLaserInterpolant = pow(sin(coords.y * 3.14) * 0.5 + 0.5, 4.0);
    float4 innerColor = lerp(glowColor, insideColor, innerLaserInterpolant) * innerLaserInterpolant;
    
    float4 mixedColor = AlphaBlend(innerColor, backGlow);
 
    mixedColor *= input.Color;
    return mixedColor;
}
float4 GlowVersion2(VertexShaderOutput input)
{
    float2 coords = input.TextureCoordinates;
    float interp = (sin(coords.y * 3.14) * 0.5 + 0.5);
    interp = smoothstep(0.0, 1.0, interp);
    float4 mix1 = lerp(bloomColor, glowColor, interp);
    float4 mix2 = lerp(glowColor, insideColor, interp);
    float4 mix3 = lerp(mix1, mix2, interp);
    mix3 *= interp;
    mix3 *= input.Color;
    return mix3;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return GlowVersion1(input);
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}