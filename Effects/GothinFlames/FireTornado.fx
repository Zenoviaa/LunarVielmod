sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float4 gradientBottomColor;
float4 gradientTopColor;
float2 resolution;
float time;

float2 SampleCoordinates(float2 coords, float2 offset)
{
    float2 sampleCoords = coords + offset;
    sampleCoords.x *= 0.3;
    sampleCoords.y *= 4.0;
    float roll = sin(coords.x * 1.5);
    sampleCoords.y += roll;
    sampleCoords += float2(time * 12.0, 0.0);
    return frac(sampleCoords);
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
    uv.y += time * 2.5;
    uv *= 4.0;
    uv.x *= 0.4;
    uv.x += time * 24.0;
    uv = frac(uv);
    uv.y += sin(coords.x * 3.14159) * 0.4;
    uv += offset;
    return uv;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{ 
 
    coords.x += sin(time * 24.0 + coords.y * 4.0) * 0.1;
    //Screen color
    float2 uv = SampleCoordinates(coords, float2(0.0, 0.0));

    float2 uv2 = SampleCoordinates2(coords, float2(0.3, -0.25));
    float2 uv3 = SampleCoordinates(coords, float2(0.6, -0.4));
    float smokeColor = tex2D(uImage1, uv3).r;
    float2 offset = float2(0.0, sin(time * 0.15) * smokeColor * 0.05);
    
    uv3.y = 1.0 - uv3.y;
    uv2 = frac(uv2);
    uv2.x = 1.0 - uv2.x;
    uv2.y += time;
    uv3.y += time;
 
    float2 puv = uv + offset;
    puv *= 4.0;
    puv = frac(puv);
    float color = tex2D(uImage1, puv).r;
    float color2 = tex2D(uImage1, uv2 + offset).r;
    float mixed = color + color2;
    float4 fireColor = lerp(gradientBottomColor, gradientTopColor, mixed);

//    float d = color.r < coords.x;
    fireColor.r += 0.8;
    fireColor = pow(fireColor, 1.4);
    fireColor.rgb -= 0.45;
    
    float n = tex2D(uImage2, uv2).r;
    float threshold = 1.0 - coords.y;
    threshold *= threshold * threshold;
    fireColor.rgb /= 2.95;
    fireColor.rgb += mixed * 0.05;
    
    fireColor *= lerp(1.5, 4.0, sin(coords.x * 3.14));
    fireColor += 0.07;
    fireColor *= 1.5;
    
    float l = lerp(-1.0, -5.0, coords.y);
    fireColor *= lerp(l, 1.0, sin(coords.x * 3.14));
    fireColor *= lerp(0.0, 1.0, sin(coords.y * 3.14));
    //fireColor *= lerp(0.0, 1.0, sin(coords.y * 25.0));
   // fireColor *= lerp(, 1.0, coords.y);
    return fireColor * sampleColor;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}