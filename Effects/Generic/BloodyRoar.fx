sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float strength;

float2 Rotate(float2 direction, float radians)
{
    float2 center = float2(0.0, 0.0);
    float num = cos(radians);
    float num2 = sin(radians);
    float2 v = direction - center;
    float2 result = center;
    result.x += v.x * num - v.y * num2;    
    result.y += v.x * num2 + v.y * num;
    return result;
}
float2 RotateCenter(float2 direction, float radians)
{
    float2 center = float2(0.5, 0.5);
    float num = cos(radians);
    float num2 = sin(radians);
    float2 v = direction - center;
    float2 result = center;
    result.x += v.x * num - v.y * num2;
    result.y += v.x * num2 + v.y * num;
    return result;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{  
    //Sampler noise
    float n = tex2D(noiseSampler, frac(coords + float2(time * -0.5, 0.0))).r;
   
    float2 uv = coords;
    
    //Rotate 90 degrees and wave that direction based on the direction from the center
    float2 dir = uv - float2(0.5, 0.5);
    float diff = length(dir);
    float alpha = saturate(diff / 0.5);
    
    float2 rotatedDir = Rotate(dir, 1.54);
    uv -= dir * n * strength * 4.0;
    
    float progress = saturate(tintColor.b / 255.0);
    float radians = 3.0 * alpha;
    
    float2 uv2 = uv; 
    
    uv = RotateCenter(uv, radians + time * -0.7);
    
    //Circular fade based on how far the texture is

    float fade = alpha;
    float fade2 = 1.0 - saturate((diff - 0.45) / 0.1);

    float4 spriteColor = tex2D(spriteSampler, uv) * tintColor * fade * 1.2 - n * 0.3;
    for (float f = 0.0; f < 4.0; f++)
    {
        uv = RotateCenter(uv, radians * (f + 1.0));
        float4 spriteColor2 = tex2D(spriteSampler, uv) * tintColor * fade * 1.2 - n * 0.3;
        spriteColor -= spriteColor2 * 0.5;
    }
    
    for (float k = 0.0; k < 2.0; k++)
    {
        uv2 = RotateCenter(uv2, 2521.0);
        float4 spriteColor2 = tex2D(spriteSampler, uv2) * tintColor * fade * 1.2 - n * 0.3;
        spriteColor += spriteColor2 * 0.4 * tintColor;
    }
   
   
    spriteColor.gb -= 0.5;
    return spriteColor * fade2 * 2.0;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}