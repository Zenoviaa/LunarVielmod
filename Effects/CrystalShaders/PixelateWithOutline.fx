sampler spriteSampler : register(s0);
float width;
float height;
float2 texelSize;

float SampleAlpha(in float2 uv)
{
    return tex2D(spriteSampler, uv).a;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    uv.x = round(uv.x * width) / width;
    uv.y = round(uv.y * height) / height;
    
    float4 spriteColor = tex2D(spriteSampler, uv);
    if (spriteColor.r == 0.0 && spriteColor.g == 0.0 && spriteColor.b == 0.0 && spriteColor.a == 0.0)
    {
        //check for outlines
        float2 left = uv + float2(-texelSize.x, 0.0);
        float2 right = uv + float2(texelSize.x, 0.0);
        float2 up = uv + float2(0.0, -texelSize.y);
        float2 down = uv + float2(0.0, texelSize.y);
        
        float a = SampleAlpha(left);
        a = max(a, SampleAlpha(right));
        a = max(a, SampleAlpha(up));
        a = max(a, SampleAlpha(down));
        return sampleColor * a;
    }
    else
    {
        return spriteColor;
    }
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};