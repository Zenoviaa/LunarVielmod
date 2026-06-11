sampler uImage0 : register(s0);
texture mixTexture;
sampler2D mixTex = sampler_state
{
    texture = <mixTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float2 texelSize;
float4 outlineColor;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float4 colorToMix = tex2D(mixTex, coords);
    float4 mask = tex2D(uImage0, coords);
    float4 mixedColor = colorToMix * sampleColor * mask;
    
    //Not sure if there's a good way to remove this branch
    //or if it's worth it
    //technically speaking i could do uh
    if (mixedColor.a > 0)
        return mixedColor;

    float2 leftCoords = coords + float2(-texelSize.x, 0.0);
    float2 rightCoords = coords + float2(texelSize.x, 0.0);
    float2 upCoords = coords + float2(0.0, -texelSize.y);
    float2 downCoords = coords + float2(0.0, texelSize.y);
    
    float4 left = tex2D(uImage0, leftCoords);
    float4 right = tex2D(uImage0, rightCoords);
    float4 up = tex2D(uImage0, upCoords);
    float4 down = tex2D(uImage0, downCoords);
    
    float outlineAlpha = left.a;
    outlineAlpha = max(outlineAlpha, right.a);
    outlineAlpha = max(outlineAlpha, up.a);
    outlineAlpha = max(outlineAlpha, down.a);
    return outlineAlpha * outlineColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}