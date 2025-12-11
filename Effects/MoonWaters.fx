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

sampler2D ClampTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = clamp;
    AddressV = clamp;
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


Texture2D NoiseTexture;
sampler2D NoiseTextureSampler = sampler_state
{
    Texture = <NoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};


struct HeightPixelShaderOutput
{
    float4 Height : SV_Target0;
    float4 Light : SV_Target1;
};

float time;
float levels;
float distortion;
float3 startGradient;
float3 endGradient;
float2 tiling;
float2 screenOffset;


float reflectionDistance;
float2 reflectionTexelSize;
float reflectionPower;

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
        
    //Distort the coordinates
    float d = tex2D(SpriteTextureSampler, coords + screenOffset);
    float2 distortionOffset = float2(sin(d), cos(d)) * distortion;
    coords *= tiling;
    coords += screenOffset;
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

HeightPixelShaderOutput HeightPS(VertexShaderOutput input)
{
    float2 coords = input.TextureCoordinates;
    float4 color = input.Color;
    
    //Calculate how many tiles down we are
    //Step 1. Calculate the depth that we would be fading to
    const float Max_Depth = 32.0;
    float heightGradient = color.a;
    float depth = heightGradient * Max_Depth;
    
    //Step 2. calculate depth of htis pixel
    float pixelDepth = depth - coords.y;
    
    //Step 3. Calculate our new alpha value
    //Make sure to invert it, low depth means it's at the surface and should be bright
    float newAlpha = pixelDepth / Max_Depth;
    
    

    HeightPixelShaderOutput output;
    output.Height = newAlpha;
    output.Light = float4(color.r, color.g, color.b, 1.0);
    return output;
}


float4 ReflectPS(VertexShaderOutput input) : COLOR
{
    //This will flip the sprite within itself
    //Here's how I think it should have to work
    
    //Step 1. Sample the current height map at this point
    float2 coords = input.TextureCoordinates;
    float heightSample = tex2D(HeightMapTextureSampler, coords).a;
    
    //Cubing the height gradient so it's not so long
    //For some reason this is faster than using pow
    float heightGradient = pow(heightSample, reflectionPower);
    
    //Step 2. Using the height map gradient, calculate how far upwards we should get the pixel
    //A value of 1 means were at the surface, so we shouldn't look that far upward
    //A value of 0 means we're at the bottom so we should go very far upward
    //We'll control this with a parameter
    float reflectionFactor = smoothstep(1.0, 0.1, heightGradient);
    float2 reflectionOffset = float2(0.0, -reflectionDistance * reflectionFactor) * reflectionTexelSize;
    
    //Step 3. Sample the new coordinates, that's our pixel
    //With how this works, it should also flip the sprite I think
    float2 reflectedCoords = coords + reflectionOffset;
    
    //Step 4. Distortion
    float d = tex2D(NoiseTextureSampler, coords + float2(time * -0.02, time * -0.04) + screenOffset).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), 0.0) * distortion;
 
    reflectedCoords += distortionOffset;
    float4 color = tex2D(ClampTextureSampler, reflectedCoords);
    
    //Step 4. blend the reflection with the height gradient so there's no reflection deep in the water
    float4 fadedColor = color * heightGradient * heightGradient;
    float4 finalColor = fadedColor * input.Color;
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

    
    float d = tex2D(SpriteTextureSampler, coords + float2(time * -0.02, time * -0.04) + screenOffset).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    distortedCoords += screenOffset;
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
    return finalColor * input.Color;
}

float4 SparklingCausticsPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float alpha = heightMapColor.a;
    alpha = pow(alpha, 4.0);
    coords *= tiling;

    
    float d = tex2D(SpriteTextureSampler, coords + float2(time * -0.02, time * -0.04) + screenOffset).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    distortedCoords += screenOffset;
    
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
    offsetCoords += coords + screenOffset;
    
    float foam = tex2D(SpriteTextureSampler, offsetCoords);
    float power = lerp(8.0, 0.3, heightMapColor.a);
    foam = pow(foam, power);
    float4 foamColor = float4(foam, foam, foam, 1.0);
    return foamColor * foam * 2.0;
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
    
    //Don't want to write if statements in a shader if possible
    //Branchless programming is best for multi-threading
    float lavaMult = baseWaterColor.r > baseWaterColor.b;
    
    //Add the water gradient to our fancy color
    //Hopefully this looks the way I want it to, I think it's gonna go to white though instead of alpha blend :sob:
    float4 fancyWaterColor = tex2D(WaterTextureSampler, coords);
    //We mneed to alpha blend the gradient on top of the base color
    
    //Let's just multiply for now
    //if this works it'll create a funny effect where there's no water 
    return fancyWaterColor * baseWaterColor.a * (1.0 - lavaMult) + baseWaterColor * lavaMult;

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
technique HeightDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL HeightPS();
    }
};
technique ReflectionDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ReflectPS();
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
