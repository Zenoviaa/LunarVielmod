sampler uImage0 : register(s0);
float threshold;

//We're taking bright colors in the original image and multiplying white so anything that's bright enough doesn't get blackened out
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
  
    //We aren't calculating the real perceived brightness of the color cause it'll change the original color
    //By taking the brightest channel we desaturate it pretty well and pretty much don't change the original color
    //Giving us a pretty accurate representation of the vanilla light map :)
    //Then we can mix it with our own lighting calculation
    float luminance = max(color.r, color.g);
    luminance = max(luminance, color.b);
    return float4(luminance, luminance, luminance, 1.0) * threshold;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};