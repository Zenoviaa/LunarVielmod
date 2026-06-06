sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
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




float4 Eyes(float2 uv)
{

    float2 diff = uv - float2(0.5, 0.5);
    

    uv += float2(time, time) * 0.2;
    uv = frac(uv);
    
    uv *= 6.0;

 
    // Time varying pixel color
    float3 col = float3(1.0, 1.0, 1.0);
    col *= sin(uv.x + time * uv.y * 6.28 * length(diff));
    col *= cos(uv.y + time * uv.x * 6.28 * length(diff));
    col += sin(time * 0.3) * 0.2;
 
    return float4(col, 1.0);
}
float4 Eyes2(float2 uv)
{

     uv *= 4.0;
    
    float distortion = tex2D(uImage1, frac(uv + float2(time * -0.15, time * -0.15))).r;
    float2 offset = float2(cos(distortion), sin(distortion)) * 0.1;
    float strength = pow(uv.x, 3.0);
    uv += float2(time * -0.05, time * -0.05);
    uv += offset;
    uv = frac(uv);

    float4 eye2 = tex2D(uImage2, uv);
    return eye2;
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
    float4 eyes = Eyes(coords);
    float eyeInterpolant = smoothstep(1.0, 0.0, pow(interp, 4.0));
    float3 ballColor = lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.0), eyeInterpolant);

    float3 mixedColor = ballColor;
    
    float exp = (1.0 - pow(interp, 8.0));
    exp *= 2.8;
  
    float4 finalColor = float4(mixedColor, 1.0) * sampleColor * exp;
    finalColor += eyes * eyeInterpolant * 0.60;
    
    float4 eyes2 = Eyes2(coords);
    finalColor += eyes2 * eyeInterpolant * 0.6;
    return finalColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};