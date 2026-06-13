sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float4 gradientBottomColor;
float4 gradientTopColor;
float time;

float2 SampleCoordinates(float2 coords, float2 offset)
{
    float2 newCoords = coords + offset;
    
    newCoords.y -= time * -5.0;
    newCoords.x += time * -3.0;
    newCoords = frac(newCoords);
    return newCoords;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Screen color
    float2 uv = SampleCoordinates(coords, float2(0.0, 0.0));
    float2 uv2 = SampleCoordinates(coords, float2(0.3, -0.25));
    float2 uv3 = SampleCoordinates(coords, float2(0.6, -0.4));
    float smokeColor = tex2D(uImage1, uv3).r;
    float2 offset = float2(0.0, sin(time * 0.15) * sin(smokeColor * 1.54 + time)  * 0.25);
    
    
    uv3.y = 1.0 - uv3.y;
    uv2 = frac(uv2);
    uv2.x = 1.0 - uv2.x;
    uv2.y -= time - coords.x + sin(time * coords.x) * 0.005;
    uv3.y += time;
 
    float color = tex2D(uImage0, uv + offset).r;;
    float color2 = tex2D(uImage0, uv2 + offset).r;
    float mixed = color + color2;
    float4 fireColor = lerp(gradientBottomColor, gradientTopColor, mixed + coords.y * 0.6);

    fireColor.r += 0.8;
    fireColor = pow(fireColor, 1.4);
    fireColor.rgb -= 0.45;

        
    float n = tex2D(uImage1, uv2).r;
    float threshold = 1.0 - coords.y;

    float d = (mixed * 0.5) > threshold;
   
   // fireColor.rgb /= 2.95;
  //  fireColor.rgb += mixed * 0.05;
    fireColor *= lerp(0.3, 2.0, coords.y + cos(coords.x * 1.54));
    fireColor += 0.07;
   
    fireColor *= d;
    fireColor.r += lerp(gradientBottomColor, gradientTopColor, coords.y).r * coords.y;
    fireColor *= 0.035;
    return fireColor * sampleColor  ;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}