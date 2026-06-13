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
    sampleCoords.x *= 0.15;
    sampleCoords.y *= 7.0;
    float roll = sin(coords.x * 3.14) * 0.25;
    roll = pow(roll, 0.5);
    sampleCoords.y -= roll;
    sampleCoords += float2(time * 14.0, 0.0);
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
    float2 baseUv = coords;
    coords.x += sin(time * 72.0 + coords.y * 12.0) * 0.12;
    //Screen color
    float2 uv = SampleCoordinates(coords, float2(0.0, 0.0));
    float2 uv2 = SampleCoordinates2(coords, float2(0.3, -0.25));
    float2 uv3 = SampleCoordinates(coords, float2(0.6, -0.4));
    float2 uv4 = SampleCoordinates(coords, float2(-0.3, 0.7));
    
    float smokeColor = tex2D(uImage1, uv3).r;
    float2 offset = float2(0.0, sin(time * 0.1 + coords.x * 4.0 + coords.y * 18.0) * smokeColor * 0.05);
    
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

    fireColor.r += 0.8;
    fireColor = pow(fireColor, 1.4);
    fireColor.rgb -= 0.45;
    
    float n = tex2D(uImage2, uv2).r;
    float threshold = 1.0 - coords.y;
    threshold *= threshold * threshold;
    fireColor.rgb /= 2.95;
    fireColor.rgb += mixed * 0.05;
    
    //First pass of color grading
    fireColor *= lerp(1.5, 6.0, sin(coords.x * 3.14));
    fireColor += 0.07;
    fireColor *= 1.5;
    
    //Variable widths at different part of the tornado to make it more violent
    float n2 = tex2D(uImage1, uv4).r;
    float r = sin(coords.y * smokeColor + coords.y * 12.0 + time * 74.0 + n2 * 6.0) * 0.5 + 0.5;
    float width = lerp(0.0, -7.0, r);
    float l = lerp(0.0, width, coords.y);
    fireColor *= lerp(l, 1.0, sin(coords.x * 3.14));
    fireColor *= lerp(0.0, 1.0, sin(coords.y * 3.14));
    fireColor *= r;
    
    //Add bright segments around the tornado
    float brightness = lerp(2.5, 4.0, sin(time * 24.0 + coords.y * 6.0) * 0.5 + 0.5);
    fireColor *= smokeColor * lerp(0.5f, brightness, (sin(time * 24.0 + coords.y * 24.0) * 0.5f + 0.5f));
    fireColor.rgb -= lerp(0.0, 0.1, saturate(sin(baseUv.y * 36.0)));
    
    //Fade it out when it gets near the edges of the texture to prevent cutting
    float diff = abs(baseUv.x - 0.5);
    float fade = diff / 0.5;
    fade = 1.0 - fade;
    fireColor *= fade;
    
 
    float4 finalColor = fireColor * sampleColor;
    //finalColor *= d;
    //finalColor = round(finalColor * 12.0) / 12.0;
    //fireColor *= lerp(0.0, 1.0, sin(coords.y * 3.14));
    //fireColor *= lerp(0.0, 1.0, sin(coords.y * 25.0));
   // fireColor *= lerp(, 1.0, coords.y);
    return finalColor;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}