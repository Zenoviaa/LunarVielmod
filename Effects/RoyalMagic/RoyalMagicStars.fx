sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float time;
float2 screenOffset;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Apply the screen offset for proper scrolling
    coords += screenOffset;
    coords = frac(coords);
    
    float2 offsetCoords = coords + float2(time * -0.05, time * -0.025);
    offsetCoords = frac(offsetCoords);
    //Rounding the coordinates so the glowing parts are more grouped together
  //  offsetCoords = round(offsetCoords * 8.0) / 8.0;
    float noise = tex2D(uImage1, offsetCoords).r;
    
    //We want to scroll the stars very slowly
    float2 starCoords = coords + float2(time * 0.005, time * 0.0025);

    starCoords = frac(starCoords);
    starCoords *= float2(4.0, 4.0);
    float stars = tex2D(uImage0, starCoords).r;
    stars *= noise + sin(time * 0.5) * 0.1;
    
    //Here we decide to use the sample color as a bloom component
    float3 starColor = lerp(sampleColor.rgb, float3(1.0, 1.0, 1.0), smoothstep(0.0, 1.0, stars)) * stars;
    float4 finalColor = float4(starColor, 1.0) * sampleColor.a;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}