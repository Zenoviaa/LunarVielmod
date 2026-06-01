sampler glowMaskSampler : register(s0);
float time;
float frequency;
float amplitude;

float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;
    //float frequency = 4.0;
    //float amplitude = 0.5;
       
    //Create a cool wave effect and then fade it out from middle outwards
    float2 vec = uv - float2(0.5, 0.5);
    float x = length(vec);
    float t = time * -1.0;
    
    float2 offset = vec * 0.1 * sin(time * frequency + x * 4.0) * amplitude;
    uv += offset;
    uv = saturate(uv);
    
    float3 col = tex2D(glowMaskSampler, uv).rgb;
    float interp = (t - x) / 1.0;
    col.rgb = lerp(col.rgb, float3(0.0, 0.0, 0.0), interp);
    
    // Output to screen
    float4 fragColor = float4(col.r, col.g, col.b, 1.0) * sampleColor;
    return fragColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};