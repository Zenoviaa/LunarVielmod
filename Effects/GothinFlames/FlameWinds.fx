sampler uImage0 : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{   
    float2 uv = coords;
    uv.x *= 0.15;
    uv.y *= 1.4;
    uv = frac(uv);
    
    float2 sampleCoords = uv + float2(time * 0.1, 0.0);
    sampleCoords.y += sin(time * 0.3 + uv.x * 9.0) * 0.02;
    sampleCoords = frac(sampleCoords);
    
    float4 flameCol = tex2D(uImage0, sampleCoords);
    float4 finalColor = flameCol * sampleColor;
    float d = finalColor.r > 0.2;
    finalColor.gb *= 0.65;
   // finalColor *= d;
 //   finalColor *= sin(coords.x * 3.14);
    finalColor *= 1.2;
    return finalColor * sampleColor;
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}