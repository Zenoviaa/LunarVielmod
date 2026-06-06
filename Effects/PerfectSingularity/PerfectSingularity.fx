sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float time;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    coords += float2(time * 0.05, time * 0.05);
    coords = frac(coords);
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    float2 polarUV = float2(angle, dist);
   
    return polarUV;
}


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Let's draw a circle
    float2 vec = coords - float2(0.5, 0.5);
    float2 polarCoords = PolarCoordinates(coords);
    float noise = tex2D(uImage1, polarCoords).r;
    float2 distortedUV = coords + vec * noise * 0.2;
    
    //Perfect singularity is just black and white
    float2 distortedVec = distortedUV - float2(0.5, 0.5);
    float interp = saturate(length(distortedVec) / 0.5);
    
    //Color variation
    //interp += sin(time * 0.05 + interp) * 0.05;
    float3 ballColor = lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.0), smoothstep(1.0, 0.0, pow(interp, 4.0)));
    float3 mixedColor = ballColor;
    
    float exp = (1.0 - pow(interp, 8.0));
    exp *= 2.8;
  
    float4 finalColor = float4(mixedColor, 1.0) * sampleColor * exp;
    return finalColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};