sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{  
    float2 diff = (coords - float2(0.5, 0.5));
    diff.x += sin(coords.x * 16.0 + time * 4.0) * 0.015;
    diff.y += cos(coords.y * 16.0 + time * 4.0) * 0.015;
  
    float2 distort = diff - diff * 0.5 * sin(-time * 4.0 + length(diff) * 8.0);
    float len = length(distort);
    float inerp = clamp(len / 0.5, 0.0, 1.0);
    float3 col = float3(inerp, inerp, inerp);
    
    float inerp2 = clamp(length(diff) / 0.5, 0.0, 1.0);
    col *= (1.0 - inerp2);
    col.r = pow(col.r, 3.0);
    col.g = pow(col.g, 3.0);
    col.b = pow(col.b, 3.0);
    col *= 16.0;

    float4 fragColor = float4(col, 1.0) * tintColor;
    return fragColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}