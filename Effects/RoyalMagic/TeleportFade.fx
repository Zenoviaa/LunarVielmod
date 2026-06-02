sampler uImage0 : register(s0);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float3 startColor = float3(1.0, 1.0, 1.0);
    float3 bloomColor = sampleColor.rgb;
 
    float y = uv.y * 3.14;
    y = sin(y);
    
    float x = uv.x * 3.14;
    x = sin(x);
    
    float interp = x * y;

    float3 col = lerp(bloomColor, startColor, interp) * interp;
    
    // Output to screen
    float4 finalColor = float4(col, 0.0) * sampleColor.a;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}