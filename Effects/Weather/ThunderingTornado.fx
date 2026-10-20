
matrix transformMatrix;
matrix rotationMatrix;
sampler spriteSampler : register(s0);
float time;
float4 tintColor;
float dir;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;

};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;
  
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 position = input.Position;
    

    float4 rotationPosition = mul(position, rotationMatrix);
    float fade = (dir * rotationPosition.z) / 64.0F;
    float osc = sin(position.y * 64.0 + time * 6.0) * 0.5 + 0.5;
    osc = lerp(0.3, 1.0, osc);
    position.xz *= lerp(0.85, 1.0, sin(position.y * 5.0 + time * 7.0) * 0.5 + 0.5);
    position.xz -= lerp(0.0, 32.0, sin(position.y + time * 6.0) * 0.5 + 0.5);
//    position.x += lerp(32.0, 0.0, position.y / 128.0);
    output.Position = mul(position, transformMatrix);

  //  output.Normal = input.Normal;
    output.Color = input.Color * fade * osc;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color * tintColor;
    float2 coords = input.TextureCoordinates.xy;
    coords = floor(coords * 16.0) / 16.0;

    float4 spriteColor = tex2D(spriteSampler, frac(coords + float2(time * 0.5, time * -2.5)));
    float4 finalColor = color * spriteColor * coords.y / 8.0;
    finalColor *= 2.4;
  //  finalColor *= input.TextureCoordinates.y / 8.0;
  //  finalColor.rgb -= coords.y / 32.0;
  //  finalColor = floor(finalColor * 4.0) / 4.0;
    //finalColor *= input.Normal.r;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
