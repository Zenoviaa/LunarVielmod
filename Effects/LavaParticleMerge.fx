#define PS_SHADERMODEL ps_3_0

sampler2D spriteSampler : register(s0);

texture maskTexture;
sampler2D maskTex = sampler_state
{
    texture = <maskTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float width;
float height;
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    uv.x = round(uv.x * width) / width;
    uv.y = round(uv.y * height) / height;
    
    
    float4 outlineColor = float4(1.0, 1.0, 1.0, 1.0);
    float2 outlineTexelSize = float2(1.0, 1.0) / (float2(width, height) * 2.0);
    outlineTexelSize *= 3.0;
    float4 mask = tex2D(maskTex, uv);
    if (mask.a <= 0.0)
    {
        float2 leftCoords = uv + float2(-outlineTexelSize.x, 0.0);
        float2 rightCoords = uv + float2(outlineTexelSize.x, 0.0);
        float2 upCoords = uv + float2(0.0, -outlineTexelSize.y);
        float2 downCoords = uv + float2(0.0, outlineTexelSize.y);
   
        float4 left = tex2D(maskTex, leftCoords);
        float4 right = tex2D(maskTex, rightCoords);
        float4 up = tex2D(maskTex, upCoords);
        float4 down = tex2D(maskTex, downCoords);
    
        if (left.a > 0)
            return left.a * outlineColor;
        if (right.a > 0)
            return right.a * outlineColor;
        if (up.a > 0)
            return up.a * outlineColor;
        return down.a * outlineColor;
    }
    
    
    float a = mask.a > 0.0;
    float4 liquid = tex2D(spriteSampler, uv) * input.Color;
    return liquid * a;
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
