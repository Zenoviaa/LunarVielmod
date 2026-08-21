sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);

//Between 0-1
float time;

//How much it's gonna push out by
float strength;

//Colors!!! Woas...
float3 innerColor;
float3 bloomColor;


sampler maskSampler : register(s2);

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{    
    float2 originalCoords = coords;
    //Distorting Big Rek Balls
    float2 distortionNoiseCoords = coords + float2(time * -0.03, 0.3);
    distortionNoiseCoords = frac(distortionNoiseCoords);
    distortionNoiseCoords *= 0.2;
    float distortingNoise = tex2D(noiseSampler, distortionNoiseCoords).r;
    
  
    float2 scrollingNoiseCoords = coords + float2(time * -0.03, 0.0);
    float scrollingNoise = tex2D(noiseSampler, scrollingNoiseCoords).r;
    scrollingNoise *= lerp(1.0, 4.0, coords.x);
    float strength = 0.08;
    coords.y += sin(distortingNoise * 3.14) * strength - strength* 0.5;
    coords.x += time * -0.05;

    coords = frac(coords);
    float yDir = (originalCoords.y - 0.5);
   // coords.y += acos(coords.x * 3.14) * 0.05;
    float4 spriteColor = tex2D(spriteSampler, coords);
  
    float3 tint = lerp(bloomColor, innerColor, coords.x);
    float mask = tex2D(maskSampler, originalCoords);

    spriteColor *= mask;
    spriteColor.rgb *= tint;
    spriteColor.rgb += originalCoords.x * spriteColor.r;
    spriteColor.rgb *= originalCoords.x;
    return spriteColor * scrollingNoise * 2.0;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}