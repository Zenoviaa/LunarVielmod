sampler uImage0 : register(s0);
float time;
float3 bloomColor;

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    dist -= time * 0.2;
    dist = frac(dist);
    float2 polarUV = float2(angle, dist);

    return polarUV;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{ 
    //Calculate edge fading, so it's circular
    float2 diff = coords - float2(0.5, 0.5);
    float l = length(diff);
    float d = saturate(l / 0.5);
    float fade = smoothstep(1.0, 0.0, d);
    
    //Mix two scrolling noises that are wrapping with polar coordinates
    float2 polarCoords = PolarCoordinates(coords);
    float2 polarCoords2 = PolarCoordinates(coords);
    
    float4 noise2 = tex2D(uImage0, frac(coords + float2(time * -0.05, time * 0.02)));
    
    
    polarCoords += float2(0.0, sin(noise2.r * 3.14) * 0.3);


    float4 noise = tex2D(uImage0, frac(polarCoords * 0.4));

    float4 combinedNoise = noise + noise2;
  
    

    
    float3 bloomingColor = lerp(bloomColor, tintColor.rgb, fade);
    noise.rgb += bloomingColor;
    noise *= tintColor.a;
    noise = pow(noise, 2.0);
    //Posterize colors before applying the fading effect so we don't get that weird banding
 //   noise = floor(noise * 2.0) / 2.0;
    noise *= fade;
    return noise;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}