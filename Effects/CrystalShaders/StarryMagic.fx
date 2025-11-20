matrix transformMatrix;



float time;
float3 glowColor;
float3 glowColor2;
float2 tiling;
texture starryTexture;
sampler2D starryTex = sampler_state
{
    texture = <starryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}


float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //First we unpack our texture with some nice scrolling
    float2 coords = input.TextureCoordinates;
    coords *= tiling;
    float2 starsOffset = float2(time * -0.05, 0.0);
    
    //At the front of the trail we want to squish on the y on a bit, based on the x
    float yStarSquish = smoothstep(1.0, 0.2, coords.x);
    float2 starCoords = coords;
    starCoords.y *= yStarSquish;

    
    //End as it gets further back the x like, stretches out
    starCoords.x *= starCoords.x;
    starCoords += starsOffset;
    float stars = tex2D(starryTex, starCoords).r;
       
    float2 spiralOffset = float2(time * -0.1, 0.0);
    float spiral = tex2D(starryTex, coords + spiralOffset).g;
    
    float2 sparkleOffset = float2(time * -0.025, 0.0);
    float sparkle = tex2D(starryTex, coords + spiralOffset).b;
    
    
    float3 colorToLerpTo = lerp(glowColor2, glowColor, coords.x);
    float3 starColor = lerp(input.Color.rgb, colorToLerpTo, stars) * stars;
    float3 spiralColor = lerp(input.Color.rgb, colorToLerpTo, spiral) * spiral;
    float3 sparkleColor = lerp(input.Color.rgb, colorToLerpTo, sparkle) * sparkle;
    
    float3 mix = saturate(starColor + spiralColor + sparkleColor);
    float4 finalColor = float4(mix, 1.0) * input.Color.a;
    finalColor *= QuadraticBump(coords.y);
    return finalColor;

}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}