
matrix transformMatrix;
matrix rotationMatrix;
sampler spriteSampler : register(s0);
sampler ditherSampler : register(s1);
float time;
float4 tintColor;
float dir;
float2 spriteSize;
float2 ditherTexelSize;

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
    
    output.Position = mul(position, transformMatrix);
    output.Color = input.Color * fade * osc;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * spriteSize * ditherTexelSize;
    float dither = tex2D(ditherSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color * tintColor;
    float2 coords = input.TextureCoordinates.xy;
    
    //this creates some cool lines across the entire tornado
    //which is indeed very cool lmao
    coords = floor(coords * 16.0) / 16.0;

    float4 spriteColor = tex2D(spriteSampler, frac(coords + float2(time * 0.5, time * -2.5)));
    float4 finalColor = color * spriteColor * coords.y / 8.0;
    finalColor *= 2.4;
    finalColor -= Dither(input.TextureCoordinates.xy) * 0.1;
    finalColor = floor(finalColor * 8.0) / 8.0;
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
