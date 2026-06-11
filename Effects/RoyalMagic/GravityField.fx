sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float time;

float SampleNoise(in float2 coords)
{
    float2 offsetCoords = coords + float2(time * 0.05, time * -0.025);
    offsetCoords = frac(offsetCoords);
    float noise = tex2D(uImage1, offsetCoords).r;
    return noise;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float flicker = sin(SampleNoise(coords * 8.0));
  
    //Make a cool little circle
    float dist = length(coords - float2(0.5, 0.5));
    float a = saturate(dist / 0.5);
    a = 1.0 - a;
    float4 circleColor = sampleColor * a * (sin(time * 0.5) * 0.5 + 1.0);
    
    
    //Get the raindrops
    float2 rainCoords = coords + float2(0.0, time * -0.5);
    rainCoords = frac(rainCoords  * 2.0);
    float4 rainColor = tex2D(uImage0, rainCoords);
    rainColor.a = 0;
    return (circleColor + rainColor * circleColor.a) * sampleColor.a * 0.4;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}