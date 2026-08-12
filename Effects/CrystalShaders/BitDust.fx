
texture spriteTexture;
sampler2D spriteTex = sampler_state
{
    texture = <spriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};


float4x4 projection;
float2 screenPosition;
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct Particle
{
    float4 Color : COLOR0;
    float4 InnerColor : COLOR1;
    float4 OuterColor : COLOR2;
    float4 trans : TEXCOORD1;
    float3 TilingOffsetRotation : TEXCOORD2;
};
    
struct VertexShaderOutput
{
    float4 Color : COLOR0;
    float4 InnerColor : COLOR1;
    float4 OuterColor : COLOR2;
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
};



float2 RotatedBy(float2 spinningpoint, float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);
    float2 vec = spinningpoint;
    float2 result = float2(0.0, 0.0);
    result.x += vec.x * num - vec.y * num2;
    result.y +=  vec.x * num2 + vec.y * num;
    return result;
}

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input, Particle p)
{
    VertexShaderOutput output;
    float4 vertexPosition = input.Position;
    
    vertexPosition.xy *= p.trans.xy;
    vertexPosition.xy = RotatedBy(vertexPosition.xy, p.TilingOffsetRotation.z);
    vertexPosition.xy += p.trans.zw;
    
    output.Position = mul(vertexPosition, projection);
    output.Color = p.Color;
    output.InnerColor = p.InnerColor;
    output.OuterColor = p.OuterColor;
    
    float2 texCoords = input.TextureCoordinates;
    texCoords.y *= p.TilingOffsetRotation.x;
    texCoords.y += p.TilingOffsetRotation.y;
    output.TextureCoordinates = texCoords;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
   // return float4(1.0, 0.0, 0.0, 1.0);
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float sample = tex2D(spriteTex, input.TextureCoordinates).r;
    float3 color = lerp(input.OuterColor, input.InnerColor, sample);
    
    //Apply Bloom
   // color = pow(color, 0.75);
    color *= input.Color;
    color *= sample;
    return float4(color * 0.8, 1.0);
}

technique SpriteDrawing
{
    pass PixelPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};