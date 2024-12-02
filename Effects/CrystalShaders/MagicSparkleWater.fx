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

sampler2D outlineTex = sampler_state
{
    texture = <noiseTexture>;
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
    const float3 a = float3(0.5, 0.5, 0.5);
    const float3 b = float3(0.5, 0.5, 0.5);
    const float3 c = float3(1.000, 1.000, 1.000);
    const float3 d = float3(0.522, 0.761, 1.000);

    return d + d * cos(6.28318 * (c * t + d));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    float2 uv0 = uv;
    float3 finalColor = float3(0.0, 0.0, 0.0);
    float4 outputColor = float4(0.0, 0.0, 0.0, 0.0);

    for (float i = 0.0; i < 4.0; i++)
    {
        uv = frac(uv * 100.0) - time * 0.03;
        float d = length(uv) * exp(-length(uv0));
        float3 col = palette(length(uv0) + i * .4 + time * .4);

        d = sin(d * 8.0 + time) / 8.0;
        d = abs(d);
        d = pow(0.01 / d, 1.2);
        finalColor += col * d;
    
        //Scrolling Noise
        float2 scrollingNoise = uv + float2(sin(4.142) * distortion, time * 0.3);
    
        //Scroll multiple directions and lerp together to make a cool effect
        float3 coll = float3(0.0, 0.0, 0.0);
        for (float f = 0.0; f < 1.0; f += 0.25)
        {
            float rot = f * 3.14;
            float speed = 0.2;
            float2 offsetCoords = uv + float2(
                sin(rot) * time * speed,
                cos(rot) * time * speed);
                           
            float colorMap = tex2D(noiseTex, offsetCoords + scrollingNoise).r;
            float3 targetCol = lerp(float3(0.263, 0.416, 0.557), float3(0.0, 0.0, 0.0), colorMap);
            coll = lerp(finalColor, targetCol, 0.3);
        }  
        outputColor = float4(coll, 1.0);
    }
    float4 sparkleWaterColor = outputColor;
      
    
    uv = input.TextureCoordinates;
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, uv + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = uv + offset + float2(time * -0.25, 0.0);
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(primaryTex, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    //Scroll the outline noise
    float osn = tex2D(outlineTex, oCoords).r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * scrollingFlamingNoise,
        noiseFlame, uv.x);
    
    //Add the outline on top
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, uv.x);
    float3 finalCol = flameCol + outlineFlameCol;
    float avg = (finalCol.r + finalCol.g + finalCol.b)/3.0;
    if (avg > threshold)
    {
        finalCol = sparkleWaterColor;
    }
    
    // Output to screen
    float4 screenColor = float4(finalCol, 1.0) * input.Color.w;
    return screenColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}