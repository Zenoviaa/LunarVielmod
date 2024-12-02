matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;
texture outlineTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;
float power;

sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D outlineTex = sampler_state
{
    texture = <outlineTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
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

float3 palette(float t)
{
    float3 a = float3(0.5, 0.5, 0.5);
    float3 b = float3(0.5, 0.5, 0.5);
    float3 c = float3(0.302, 0.859, 1.000);
    float3 d = float3(1.000, 1.000, 1.000);

    return a + c * cos(2.28318 * (d * t + a));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    float2 uv0 = uv;
    float3 finalColor = float3(0.0, 0.0, 0.0);
    
    for (float i = 0.0; i < 6.0; i++)
    {
        uv = frac(uv / 0.5) - 0.5;

        float d = length(uv) * exp(-length(uv0));

        float3 col = palette(length(uv) + i * .4 + time * .4);

        d = cos(d * 10. + time) / 3.;
        d = abs(d);

        d = pow(0.01 / d, 1.1);

        finalColor += col * d;
    }
        
    float4 duutColor = float4(finalColor, 1.0);
    
    // Normalized pixel coordinates (from 0 to 1)
    float2 coords = input.TextureCoordinates;
    
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(primaryTex, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    
    //Scroll the outline noise
    float osn = tex2D(outlineTex, oCoords).r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * duutColor.rgb * scrollingFlamingNoise,
        noiseFlame, coords.x);
    
    //Add the outline on top
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, coords.x);
    float3 finalCol = flameCol + outlineFlameCol;
    
    // Output to screen
    float4 f = float4(finalCol, 1.0) * input.Color.w;
    return f;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}