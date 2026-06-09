sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float4 gradientBottomColor;
float4 gradientTopColor;
float2 resolution;
float time;

float2 SampleCoordinates(float2 coords, float2 offset)
{
    float curveFactor = 1.2;
    float2 offsetPosition = float2(0.5, 0.0);
    float2 bentCoords = float2(coords.x + offsetPosition.x * 0.5 / resolution.x, coords.y - pow(coords.x - 0.5 - offsetPosition.x / resolution.x, 2) * (coords.y - 0.5 - offsetPosition.y / resolution.y) * curveFactor);
    float roll = sin(time * 6.28 + bentCoords.x * 3);
    float2 uv2 = bentCoords + resolution * 1.2 + float2(time * 3, roll * 0.1 + offsetPosition.y / resolution.y * 0.5);
    uv2 *= 4.8;
    uv2 += offset;
    return uv2;
}
float2 SampleCoordinates2(float2 coords, float2 offset)
{
    float2 uv = coords;
    uv *= resolution.y / resolution.x;

    //Inside of a sphere??
    float scale = 1.2;
    float halfScale = scale * 0.5;
    uv = scale * uv - halfScale;
   // uv.y = asin(uv.y);

    uv.x = sin(uv.x / cos(uv.y)) - time * 0.7;
    uv.y += time * 0.5;
    uv *= 4.0;
    uv.x *= 1.45;
    uv = frac(uv);
    uv.y += sin(coords.x * 3.14159) * 0.4;
    uv += offset;
    return uv;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    coords.x = 1.0 - coords.x;
    float2 uv = SampleCoordinates(coords, float2(0.0, 0.0));
    float2 uv2 = SampleCoordinates(coords, float2(0.3, -0.25));
    float2 uv3 = SampleCoordinates(coords, float2(0.6, -0.4));
    
    uv3.y = 1.0 - uv3.y;
    uv2 = frac(uv2);
    uv2.x = 1.0 - uv2.x;
    uv2.y += time;
    uv3.y += time;
 
    float color = tex2D(uImage0, uv).r;
    float color2 = tex2D(uImage0, uv2).r;
    float smokeColor = tex2D(uImage0, uv3).r;
    float mixed = color + color2;
    float4 fireColor = lerp(gradientBottomColor, gradientTopColor, mixed);
    
    float n = tex2D(uImage1, uv2).r;
    return fireColor * sampleColor * 0.4;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}