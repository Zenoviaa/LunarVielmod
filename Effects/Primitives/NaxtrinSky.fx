sampler uImage0 : register(s0);

texture uTexture0;
sampler tex0 = sampler_state
{
    texture = <uTexture0>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uTexture1;
sampler tex1 = sampler_state
{
    texture = <uTexture1>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
texture uColorMap;
float uColorMapSection;
sampler colorMap = sampler_state
{
    texture = <uColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


float4 ColorMap(float strength)
{
    return tex2D(colorMap, float2(clamp(strength, 0.01, 0.99), uColorMapSection));
}

float2 uScreenSize;
float2 uWorldPosition;
float2 uOffsetPosition;
float uTime;
float uCurveFactor;
float uStrength;
float uPower;

float4 PixelShaderFunction(float4 baseColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float screenRatio = uScreenSize.y / uScreenSize.x * 2;

    float2 bentCoords = float2(coords.x + uOffsetPosition.x * 0.5 / uScreenSize.x, coords.y - pow(coords.x - 0.5 - uOffsetPosition.x / uScreenSize.x, 2) * (coords.y - 0.5 - uOffsetPosition.y / uScreenSize.y) * uCurveFactor);
    float roll = sin(uTime * 6.28 + bentCoords.x * 3);
    float roll2 = sin(uTime * 6.28 + bentCoords.x * 2);
    
    float noise = tex2D(tex0, bentCoords + uWorldPosition / uScreenSize * 1.2 + float2(uTime * 3, roll * 0.1 + uOffsetPosition.y / uScreenSize.y * 0.5));
    float cloud = tex2D(tex1, bentCoords * 1.1 + uWorldPosition / uScreenSize + float2(uTime * 2, roll2 * 0.2 + uOffsetPosition.y / uScreenSize.y) + (noise - 0.5) * 0.1) * (0.7 + noise * 0.3);
    
    float color = pow(cloud * uStrength, uPower);
    return float4(ColorMap(color).rgb, length(color) / 3) * baseColor;

}

technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
