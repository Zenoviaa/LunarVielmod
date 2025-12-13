matrix transformMatrix;
float time;
float2 tiling;
float3 innerColor;
float3 outerColor;

texture laserTexture;
sampler2D laserTex = sampler_state
{
    texture = <laserTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
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

float Posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    float xAlpha = QuadraticBump(uv.x);
    float yAlpha = QuadraticBump(uv.y);
    float edgeFade = xAlpha * yAlpha;
    
    uv.x = Posterize(uv.x, 128.0);
    
    float2 normalUv = uv;
    uv *= tiling;
  
    float2 offsetUv1 = uv + float2(0.1, time * -0.1);
    float3 col = tex2D(laserTex, offsetUv1).rgb;    
        
    float2 offsetUv2 = uv - float2(time * 0.09, time * 0.05);
    float3 col2 = tex2D(laserTex, offsetUv2).rgb;
    
    float3 mixedCol = lerp(col, col2, 0.5);
        
    const float levels = 4.0;
    const float power = 5.0;
    const float startPower = 0.5;
    float interp = normalUv.x * normalUv.x;
    
    mixedCol.r = pow(mixedCol.r, lerp(startPower, power, interp));
    mixedCol.g = pow(mixedCol.g, lerp(startPower, power, interp));
    mixedCol.b = pow(mixedCol.b, lerp(startPower, power, interp));
          
    mixedCol.r = Posterize(mixedCol.r, levels);
    mixedCol.g = Posterize(mixedCol.g, levels);
    mixedCol.b = Posterize(mixedCol.b, levels);
       
    mixedCol.rgb = lerp(outerColor, innerColor, mixedCol.r);
    

    return float4(mixedCol, 1.0) * input.Color * edgeFade;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}