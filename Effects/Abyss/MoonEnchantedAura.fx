sampler spriteSampler : register(s0);
float time;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    dist -= time * 0.2;
    dist = frac(dist);
    float2 polarUV = float2(angle, dist);

    return polarUV;
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

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
   
    float2 diff = coords - float2(0.5, 0.5);
    float len = length(diff);
    float a = saturate(len / 0.5);
    float a2 = pow(a, 6.0);
    
    
    float2 rotatedCoords = RotateCenter(diff, a * 3.14);
    float2 polarCoords = PolarCoordinates(rotatedCoords);
    polarCoords.x -= time;
    float4 noiseColor = tex2D(spriteSampler, polarCoords);
    noiseColor *= smoothstep(1.0, 0.0, a);
    noiseColor *= sampleColor;

    noiseColor *= a2;
    noiseColor *= 90.0;
    noiseColor *= smoothstep(0.0, 1.0, pow(a, 3.0));
    return noiseColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
