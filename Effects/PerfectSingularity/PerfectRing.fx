sampler uImage0 : register(s0);
float time;


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    float2 vec = uv - float2(0.5, 0.5);
    float d = length(vec) / 0.5;
    d = clamp(d, 0.0, 1.0);

    float3 col = float3(1.0, 1.0, 1.0);
    float x = sin(d * 12.5663706 + sin(time + d * 8.0));
    x = pow(x, -0.78);
 //   x = saturate(x);
    float fade = 1.0 - d;
    col *= x;
    
    
    col *= uv.y;
    
    float angle = atan2(vec.y, vec.x);
    float fade2 = saturate(angle / 2.0);
    float fade3 = d;
    float4 finalColor = float4(col, 1.0) * sampleColor * fade * pow(d, 2.0) * lerp(-1.0, 1.0, uv.y);
    return finalColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};