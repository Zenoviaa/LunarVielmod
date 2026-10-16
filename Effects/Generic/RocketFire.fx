sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler ditherSampler : register(s2);
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

float2 ditherTexelSize;
float2 spriteSize;
float2 screenOffset;


float Dither(float2 screenUV)
{
    //Here we multiple the screen uv by the image size to get it back to 0-1, and then multiple by the texel size of the dither to normalize it
    float2 ditherTextureUV = screenUV * spriteSize * ditherTexelSize;
    ditherTextureUV += screenOffset;
    float dither = tex2D(ditherSampler, ditherTextureUV).r;
    return dither;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 scrollingCoords = coords + float2(time * 1.5, 0.0);

    scrollingCoords = frac(scrollingCoords);

    float noise = tex2D(noiseSampler, scrollingCoords).r;
    noise *= coords.x;

    float mask = tex2D(spriteSampler, coords).r;
    float alpha = noise * mask;
    float3 color = lerp(float3(1.0, 1.0, 1.0), tintColor.rgb, mask);
    float4 finalColor = float4(color, 1.0) * alpha;
    finalColor *= coords.x * 7.0;
    finalColor = pow(finalColor, 2.5);

    float ditherColor = Dither(coords );
    finalColor.rgb -= ditherColor ;
    finalColor.rgb = saturate(finalColor.rgb);
    
    finalColor.rgb *= 0.99;
    finalColor *= coords.x;
    float4 colorToMapTo = tex3D(ColorSpectrumTextureSampler, finalColor.rgb);
    finalColor.rgb = colorToMapTo.rgb;
    
    return finalColor * tintColor.a * mask * lerp(0.0, 2.0, pow(coords.x, 4.0));
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}