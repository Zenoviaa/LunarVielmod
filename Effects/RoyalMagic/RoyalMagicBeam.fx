sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float time;
float2 tiling;
float3 bloomColor;
float distortion;

float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float2 SampleDistortionOffset(float2 coords)
{
    float2 distCoords = frac(coords + float2(time * -0.05, time * -0.025));
    float n = tex2D(uImage1, distCoords);
    float radians = n * 6.28;
    float2 offset = float2(cos(radians), sin(radians));
    float2 distortionOffset = offset * distortion;
    return distortionOffset;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Sample the distortion
    float2 laserCoords = coords + float2(time * -0.05, 0.0);
    laserCoords += SampleDistortionOffset(coords);
    laserCoords = frac(laserCoords);
    laserCoords *= tiling;
    float4 laserColor = tex2D(uImage0, laserCoords);
    
    //Calculate Bloom
    float osc = smoothstep(0.0, 0.5, sin(time * 2.3) * 0.5 + 0.5);
    float bump = QuadraticBump(coords.y);
    float3 bloom = lerp(float3(0.0, 0.0, 0.0),bloomColor, bump);
  //  bloom *= 1.5;
    laserColor.rgb += bloom;
    
    //Multiply input colorr
    laserColor *= sampleColor;
    return laserColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}