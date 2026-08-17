sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);

//Between 0-1
float time;

//How much it's gonna push out by
float strength;

//Colors!!! Woas...
float3 innerColor;
float3 bloomColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 origin = float2(0.0, 0.5);
    float2 diff = coords - origin;
    float2 normalDiff = normalize(diff);
    float2 pushedCoords = coords + normalDiff * strength * time;
    float2 noiseCoords = coords + float2(time * 5.0, 0.0);
    noiseCoords = frac(noiseCoords);
    
    float n = tex2D(noiseSampler, noiseCoords);
    float outward = saturate(length(diff) / 0.5);
    pushedCoords.y += sin(n * 6.28) * 0.25;
    
    float4 spriteColor = tex2D(spriteSampler, pushedCoords);
    spriteColor.rgb *= lerp(innerColor, bloomColor, outward);
    spriteColor *= lerp(1.0, 0.0, outward );
    return spriteColor * sampleColor * 2.0;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}