sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float2 resolution;
float time;
float2 SampleCoordinates2(float2 coords, float2 offset)
{
    float2 uv = coords;
    uv *= resolution.y / resolution.x;

    //Inside of a sphere??
    float scale = 1.2;
    float halfScale = scale * 0.5;
    uv = scale * uv - halfScale;
    uv.x = sin(uv.x / cos(uv.y)) - time * 0.7;
    uv.y += time * 3.5;
    uv *= 4.0;
    uv.x *= 0.4;
    uv.x += time * 24.0;
    uv = frac(uv);
    uv.y -= sin(coords.x * 9.8) * 0.2;
    uv += offset;
    return uv;
}

float SampleNoise(float2 coords)
{
    float2 offsetCoords = frac(coords + float2(time, time * 0.5));
    float n = tex2D(noiseSampler, offsetCoords);
    return n;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 baseUv = coords;
    baseUv.x += sin(time * 120.0 + coords.y * 8.0) * 0.2;
    float2 uv2 = SampleCoordinates2(baseUv * float2(0.2, 1.0), float2(0.3, -0.25));

    float4 color = tex2D(noiseSampler, uv2);

    float alpha = sin(coords.x * 3.14);
    float alpha2 = sin(coords.y * 3.14);
    float4 windColor = lerp(float4(1.0, 1.0, 1.0, 1.0), sampleColor, color.r);
    float4 finalColor = windColor * alpha * color.r * alpha2;
    
    float2 diff = float2(0.5, 1.0) - coords;
    float l = length(diff);
    float a = 1.0 - saturate(l / 0.5);
    finalColor *= 1.3;
    finalColor *= coords.y * 1.8;
    
    float n = SampleNoise(coords * float2(1.0, 2.0));
    finalColor *= sin(time * 28.0 + n * 3.0) * 3.0 + 5.5;
    finalColor *= (a + 0.5);
    finalColor *= 0.4;
    finalColor *= sampleColor.a;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}