matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;
texture shapeTexture;

float3 primaryColor;
float3 noiseColor;

float time;
float distortion;
float threshold;

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

sampler2D shapeTex = sampler_state
{
    texture = <shapeTexture>;
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

float3 LeavesPixelShaderFunction(float2 coords)
{
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    
    //Multiply by coords.x to have back leaves distort more
    float offset = s * distortion * (1.0 - coords.x);
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    //Calculate Leaves
    float leavesSample = tex2D(primaryTex, oCoords);
    float3 leavesColor = leavesSample * primaryColor;
    return leavesColor;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Normalized pixel coordinates (from 0 to 1)
    float2 coords = input.TextureCoordinates;
    
    //Calculate Leaves
    float3 leavesColor = LeavesPixelShaderFunction(coords);
      
    //Scroll the noise
    float s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    float shapeSample = tex2D(shapeTex, oCoords);
    float3 shapeColor;
    if (shapeSample > threshold)
    {
        shapeColor = leavesColor * input.Color.rgb;
    }
    else
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }

    // Output to screen
    float4 finalColor = float4(shapeColor, 1.0) * input.Color.w;
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