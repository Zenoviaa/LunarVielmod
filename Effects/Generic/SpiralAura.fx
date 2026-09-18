sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float spiralStrength;
float4 bloomColor;
///Rotates around a specific point
float2 Rotate(float2 vec, float2 center, float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);
    float2 v = vec - center;
    float2 result = center;
    result.x += v.x * num - v.y * num2;
    result.y += v.x * num2 + v.y * num;
    return result;
}

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

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    //Spiral the texture even more 
    float2 diff = coords - float2(0.5, 0.5);
    float alpha = saturate(length(diff) / 0.5);
    float2 rotatedUV = Rotate(coords, float2(0.5, 0.5), spiralStrength * alpha + time);
    
    rotatedUV += diff * 0.5f * sin(-time * 16.0 + alpha * 8.0) * 4.0;
    float4 spiralColor = tex2D(spriteSampler, rotatedUV);
    spiralColor += bloomColor * alpha * spiralColor.r;

    //Should make blobs moving outward
    float2 polarCoords = PolarCoordinates(coords);
    polarCoords.y -= time * 2.0;
    float4 noiseColor = tex2D(noiseSampler, polarCoords * 0.2);
    noiseColor.a = 0;
    
    float a = alpha;
    noiseColor *= (1.0 - a);
    
    spiralColor += noiseColor * spiralColor.a * 3.0;
    spiralColor *= tintColor;
    spiralColor *= (1.0 - a);
    return spiralColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}