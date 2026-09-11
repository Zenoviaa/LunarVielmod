sampler spriteSampler : register(s0);
float time;
float2 texelSize;
float4 cloudColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    uv.x *= 0.67;
    uv *= 3.0; 
    float2 noiseUV = uv + float2(time, time * 0.5);
    noiseUV = frac(noiseUV);
    float4 spriteColor = tex2D(spriteSampler, noiseUV);
    
    float2 noiseUV2 = uv + float2(time + 0.2, time * -0.5);
    noiseUV2 = frac(noiseUV2);
    float4 spriteColor2 = tex2D(spriteSampler, noiseUV2);
    
    float4 mixedColor = spriteColor + spriteColor2;
    mixedColor *= 0.5;
    mixedColor = floor(mixedColor * 8.0) / 8.0;

    
    float4 alphas;
    float2 pixelSize = texelSize * 16.0;
    alphas.x = saturate(coords.x / pixelSize.x);
    alphas.y = saturate(coords.y / pixelSize.y);
    alphas.w = saturate((1.0 - coords.x) / pixelSize.x);
    alphas.z = saturate((1.0 - coords.y) / pixelSize.y);
    return mixedColor * tintColor * alphas.x * alphas.y * alphas.w * alphas.z;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}