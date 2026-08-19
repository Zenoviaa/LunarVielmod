sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);

//Between 0-1
float time;

//How much it's gonna push out by
float strength;

//Colors!!! Woas...
float3 innerColor;
float3 bloomColor;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float2 polarUV = float2(angle, dist);
    return polarUV;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 pCoords = PolarCoordinates(coords) + float2(0.0, time * -0.05);
    float2 diff = coords - float2(0.5, 0.5);
    
    //Create a glowball
    float dist = length(diff);
    float ratio = dist / 0.5;
    float progress = saturate(ratio);
    float3 color = lerp(innerColor, bloomColor, progress);
    
    //Fafde out
    float alpha = smoothstep(1.0, 0.0, progress);
    float noise = tex2D(noiseSampler, pCoords);
    
    //Create detail/texturing in the colors
    color -= noise * 0.2;
    return float4(color, 1.0) * sampleColor * alpha;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}