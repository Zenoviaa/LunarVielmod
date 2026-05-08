sampler uImage0 : register(s0);
float width;
float height;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    uv.x = floor(uv.x * width) / width;
    uv.y = floor(uv.y * height) / height;
    return tex2D(uImage0, uv) * sampleColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};