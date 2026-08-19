sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler whirlyNoiseSampler : register(s2);

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
float2 Rotate(float2 uv, float2 pivot, float angle)
{
    //rotation matrix
    float2x2 rotation = float2x2(
            float2(sin(angle), -cos(angle)),
			float2(cos(angle), sin(angle)));
    
    uv -= pivot;
    uv = mul(uv, rotation);
    uv += pivot;
    return uv;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 pCoords = PolarCoordinates(coords * 0.2) + float2(time * 0.1, time * -0.2);
    float noise = tex2D(noiseSampler, pCoords);
    
    
    float2 whirlyNoiseCoords = pCoords + float2(0.0, -time * 0.1);
    float whirlN = tex2D(whirlyNoiseSampler, pCoords * 3.3).r;
    
    float2 diff = coords - float2(0.5, 0.5);
    
    
    coords = Rotate(coords, float2(0.5, 0.5), sin(noise * 3.2 + time));
    noise = tex2D(noiseSampler, frac(pCoords * 2.0));

    //Create a glowball
    float dist = length(diff);
    float ratio = dist / 0.5;
    float progress = saturate(ratio);
    float3 color = lerp(innerColor, bloomColor, progress);
    
    //Fafde out
    float alpha = smoothstep(1.0, 0.0, progress);
   
    
    //Create detail/texturing in the colors
    color -= noise * 0.5;
    color += whirlN * 0.8;
    color = floor(color * 4.0) / 4.0;
    return float4(color, 1.0) * sampleColor * alpha * 2.8;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}