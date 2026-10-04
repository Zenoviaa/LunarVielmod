
float2 PolarCoordinates(in float2 coords)
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

float2 Rotate(in float2 direction, in float radians)
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

float2 RotateAround(in float2 direction, in float2 center, in float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);
    float2 v = direction - center;
    float2 result = center;
    result.x += v.x * num - v.y * num2;
    result.y += v.x * num2 + v.y * num;
    return result;
}

float CircleRatioFromCenter(in float2 coords)
{
    float d = length(coords - float2(0.5, 0.5));
    float s = saturate(d / 0.5);
    return s;
}

float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}