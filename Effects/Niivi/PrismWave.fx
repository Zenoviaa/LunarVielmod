#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler gradientSampler : register(s2);
sampler ditherSampler : register(s3);
float2 spriteSize;
float2 ditherTexelSize;
float time;

Texture3D ColorSpectrumTexture;
sampler3D ColorSpectrumTextureSampler = sampler_state
{
    Texture = <ColorSpectrumTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0, float4 pos : SV_Position) : COLOR0
{
    float2 ditherCoords = coords * spriteSize * ditherTexelSize;
    float4 ditherColor = tex2D(ditherSampler, ditherCoords);
  
    float offset = tintColor.r * 12.0;
    float pixelated = 128.0;
    coords = floor(coords * pixelated) / pixelated;

    coords.x += sin(coords.y * 7.0 + time * 7.0 + offset) * 0.07;
    float2 noiseCoords = coords + float2(0.0, time * 2.0);
    noiseCoords *= 8.0;
    float4 noiseSampleColor = tex2D(noiseSampler, float2(noiseCoords.y, noiseCoords.x));
 
    
    //WHOAAA PRISMATIC!!!
    float4 color = float4(1.0, 1.0, 1.0, 1.0);
    color *= coords.y;
    float4 rainbowColor2 = tex2D(gradientSampler, float2(noiseSampleColor.r, 0.0));
 //   rainbowColor2.rgb = lerp(rainbowColor2.rgb, float3(1.0, 1.0, 1.0), 0.5);
   
    color -= rainbowColor2 * 0.15;
    color *= noiseSampleColor;
    color += 0.5;
    color *= sin(coords.y * 8.0 + time * 4.0 + offset) * 0.05;
   // color *= QuadraticBump(coords.x);
 
  //  color += tintColor.a * coords.y;
    //color *= coords.y;
    color += coords.y;
    
    float range = 0.3;
    color *= saturate(coords.x / range);
    color *= saturate((1.0 - coords.x) / range);
    color *= saturate((1.0 - coords.y) / 0.1);
    color.r += sin(offset + coords.x * 4.0) * 0.2;
    color.g += cos(offset + coords.x * 8.0) * 0.2;
    color.b += sin(offset + coords.x * 2.0) * 0.2;
//    color *= sin(coords.x * 16.0 + time * 6.0) * 4.0;
    //color = floor(color * 8.0) / 8.0;
    float4 woaColor = tex2D(gradientSampler, float2(frac(offset + coords.x), 0.2));
   // return woaColor;
    float4 rainbowColor = tex2D(gradientSampler, float2(frac(pow(1.0 - coords.y, 0.6) - time - offset), coords.y));
    float4 rainbowColor3 = tex2D(gradientSampler, float2(frac(coords.x + time * 4.0 + offset), coords.y));
    color *= rainbowColor;

  //  color += saturate(coords.y - 0.65) / 0.5 * color.a;
    color *= tintColor.a;


    float4 painterlyColor = tex2D(noiseSampler, coords * float2(5.0, 5.0) + float2(sin(coords.x * 2.0), time * 2.0));
    float4 rainbowColor4 = tex2D(gradientSampler, float2(painterlyColor.r, coords.y));
   // painterlyColor = floor(painterlyColor * 8.0) / 8.0;
    painterlyColor = floor(painterlyColor * 8.0) / 8.0;
 //   color *= painterlyColor * 3.0;
 //   color += sin(coords.y * 4.0 + time * 32.0 + offset) * color.a * 0.2; //tex2D(gradientSampler, float2(coords.y, 0.0)) * color.a * 0.6;
    color += noiseSampleColor * 0.2 * color.a;
    color += ditherColor * 0.6 * color.a;
    
    color.rgb = saturate(color.rgb);
    
    //The colors bug out if it ever reaches 1, so we need to just make it barely under
    //Smh this is stupid, so the bug was with the texture sampling.
    color.rgb *= 0.99;
    
    float4 newColor = color * tex2D(gradientSampler, float2(frac(time * -0.15 + coords.x + offset), 0.0));
    color = lerp(color, newColor, 0.6);

    //color += sin(time * 4.0 + coords.x * 8.0 + offset) * color.a * 0.2;
    float4 colorToMapTo = tex3D(ColorSpectrumTextureSampler, color.rgb);
    //color += rainbowColor4 * 0.15;
  
  //  color = floor(color * 4.0) / 4.0;
    //color *= tintColor;

    colorToMapTo.a = 0.0;
    return colorToMapTo ;
    //return coords.y * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}