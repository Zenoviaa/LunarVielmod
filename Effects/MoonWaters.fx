#define PS_SHADERMODEL ps_3_0

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = wrap;
    AddressV = wrap;
};

Texture2D HeightMapTexture;
sampler2D HeightMapTextureSampler = sampler_state
{
    Texture = <HeightMapTexture>;
};
Texture2D WaterTexture;
sampler2D WaterTextureSampler = sampler_state
{
    Texture = <WaterTexture>;
    AddressU = clamp;
    AddressV = clamp;
};


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float time;
float levels;
float distortion;
float3 startGradient;
float3 endGradient;
float2 tiling;

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
        
    //Distort the coordinates
    float d = tex2D(SpriteTextureSampler, coords);
    float2 distortionOffset = float2(sin(d), cos(d)) * distortion;
    coords *= tiling;
    coords += distortionOffset;
    
    float2 offset = float2(time * -0.05, 0.0);
    float2 offset2 = float2(time * 0.05, 0.1);
    
    float4 sample1 = tex2D(SpriteTextureSampler, coords + offset);
    float4 sample2 = tex2D(SpriteTextureSampler, coords + offset2);
    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color * input.Color;
    
    finalColor.r = posterize(finalColor.r, levels);
    finalColor.g = posterize(finalColor.g, levels);
    finalColor.b = posterize(finalColor.b, levels);
    return finalColor;
}




float4 GradientPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
  
    float4 finalColor = tex2D(SpriteTextureSampler, coords);
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float3 gradient = lerp(endGradient, startGradient, heightMapColor.a);
    gradient *= gradient;
    finalColor.rgb *= gradient;
    return finalColor;
}


float4 CausticsPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    coords *= tiling;

    float d = tex2D(SpriteTextureSampler, coords + float2(time * -0.02, time * -0.04)).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    float2 offset = float2(time * -0.05, 0.0);
    float2 offset2 = float2(time * 0.05, 0.3);
    
    float4 sample1 = tex2D(SpriteTextureSampler, distortedCoords + offset);
    float4 sample2 = tex2D(SpriteTextureSampler, distortedCoords + offset2);
    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color * input.Color;
    return finalColor;
}
float4 PosterizePS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 finalColor = tex2D(SpriteTextureSampler, coords);
    finalColor.r = posterize(finalColor.r, levels);
    finalColor.g = posterize(finalColor.g, levels);
    finalColor.b = posterize(finalColor.b, levels);
    return finalColor;
}

float4 SparklingCausticsPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float alpha = heightMapColor.a;
    alpha = pow(alpha, 4.0);
    coords *= tiling;

    float d = tex2D(SpriteTextureSampler, coords + float2(time * -0.02, time * -0.04)).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    float2 offset = float2(time * -0.05, 0.0);
    float2 offset2 = float2(time * 0.05, 0.3);
    
    float4 sample1 = tex2D(SpriteTextureSampler, distortedCoords + offset);
    float4 sample2 = tex2D(SpriteTextureSampler, distortedCoords + offset2);

    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color * input.Color;
    finalColor.rgb *= alpha;
    return finalColor;
}

float4 FoamPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float2 offsetCoords = (coords * tiling) + float2(0.0, time * -0.05);
    float foam = tex2D(SpriteTextureSampler, offsetCoords);
    float power = lerp(8.0, 0.3, heightMapColor.a);
    foam = pow(foam, power);
    float4 foamColor = float4(foam, foam, foam, 1.0);
    return foamColor * foam;
}


float4 CombinePS(VertexShaderOutput input) : COLOR
{
    //Combine Water Target, Water Shader, and Height Map
    //For now let's test the height map
    float2 coords = input.TextureCoordinates;
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
       
    float3 gradient = lerp(endGradient, startGradient, heightMapColor.a);
    
    //First let's calculate the gradient
    float4 baseWaterColor = tex2D(SpriteTextureSampler, coords);
    
    //Add the water gradient to our fancy color
    //Hopefully this looks the way I want it to, I think it's gonna go to white though instead of alpha blend :sob:
    float4 fancyWaterColor = tex2D(WaterTextureSampler, coords);
    //We mneed to alpha blend the gradient on top of the base color
    
    //Let's just multiply for now
    //if this works it'll create a funny effect where there's no water 
    return fancyWaterColor * baseWaterColor.a;

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

technique GradientDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL GradientPS();
    }
};

technique CausticsDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL CausticsPS();
    }
};
technique SparklingCausticsDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL SparklingCausticsPS();
    }
};
technique FoamDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL FoamPS();
    }
};
technique PosterizeDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL PosterizePS();
    }
};
technique CombineRTDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL CombinePS();
    }
};
