sampler uImage0 : register(s0);
float2 uImageOffset;


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //All we have to do is sample the white to black of the texture, using that as an interpolant for the colors
    //Then using the time we can oscillate and add some glow with power?
    float2 offsetCoords = coords + uImageOffset;
    
    //Parallaxing
    float2 normalCoords = float2(frac(offsetCoords.x), frac(offsetCoords.y));
    float4 backgroundColor = tex2D(uImage0, normalCoords) * sampleColor;
    return backgroundColor;

}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};