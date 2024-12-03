matrix transformMatrix;
texture trailTexture;
texture distortionTexture;
texture morphTexture;
texture innerTexture;
float2 uImageSize0;
float4 uSourceRect;
float time;
float distortion;
float oscSpeed;
float3 trailColor;
float3 glowColor;
float px;
sampler2D trailTex = sampler_state
{
    texture = <trailTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D morphTex = sampler_state
{
    texture = <morphTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D distortionTex = sampler_state
{
    texture = <distortionTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D innerTex = sampler_state
{
    texture = <innerTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 innerTexSize;


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

float colorDistance2(float3 a, float3 b)
{
    float ar = abs(b.r - a.r);
    float ag = abs(b.g - a.g);
    float ab = abs(b.b - a.b);
    float d = ar + ag + ab;
    return d;
}

float3 calculateColor(float3 color)
{
	// Palette 1
    const float3 colors[5] =
    {
        float3(0.0, 0.0, 0.0),
        float3(1.0, 1.0, 1.0),
        float3(131.0/255.0, 231.0/255.0, 234.0/255.0),
        float3(6.0 / 255.0, 255.0 / 255.0, 255.0 / 255.0),
        float3(0.0 / 255.0, 0.0 / 255.0, 125.0 / 255.0)
    };

    float3 selectedColor = colors[0];
    float dist = colorDistance2(color, colors[0]);
    float currentDist;

    // For loop with the same loops than the color palette.
    for (int i = 1; i < 5; i++)
    {
        currentDist = colorDistance2(color, colors[i]);
        if (currentDist < dist)
        {
            dist = currentDist;
            selectedColor = colors[i];
        }
    }
    
    return selectedColor;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates;
    coords = round(coords / px) * px;
    
    //Calculate Distorting Noise
    float s = tex2D(distortionTex, coords + float2(round(time * -0.5 * coords.x * time), time)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    //Calcualte this goof
    float2 st = oCoords;
    float4 sampleColor = tex2D(trailTex, st + float2(time, 0.0));
    
    float2 morphCoords = st + float2(time * 1.5, 0.0);
    float4 morphColor = tex2D(morphTex, morphCoords);

    float progress = coords.x / 1.0;
    
    float4 finalColor;
    if (sampleColor.a == 0)
    {
        finalColor = morphColor;
    }
    else
    {
        finalColor = sampleColor;
    }
    
    
    float4 trueFinalColor = tex2D(innerTex, oCoords);
    trueFinalColor *= finalColor * input.Color;
    
    
    float2 o = coords - float2(0.5, 0.5);
    float l = length(o);
    trueFinalColor.rgb *= (glowColor * 2.0 * (1.0 - l));
    trueFinalColor.rgb = calculateColor(trueFinalColor.rgb);
    return trueFinalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}