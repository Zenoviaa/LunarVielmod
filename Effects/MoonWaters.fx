#define PS_SHADERMODEL ps_3_0

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D ClampTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
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

sampler brightenNoiseSampler : register(s1);
sampler causticsNoiseSampler : register(s2);
sampler foamNoiseSampler : register(s3);
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
float4 causticsColor;
float4 outlineColor;
float2 outlineTexelSize;
float2 tiling;
float2 screenOffset;
float foamLava;

float reflectionDistance;
float2 reflectionTexelSize;
float reflectionPower;

float posterize(float v, float k)
{
    return ceil(v * k) / k;
}


float4 SampleSpriteNoise(in VertexShaderOutput input, sampler2D SpriteSampler)
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

float4 SampleCausticsNoise(in VertexShaderOutput input, sampler2D SpriteSampler)
{
    float2 coords = input.TextureCoordinates;
    coords *= tiling * 3.0;
    
    float t = time;
    t *= 2.0;
    
    float d = tex2D(SpriteSampler, coords + float2(t * -0.02, t * -0.04)).r;
    float rotOffset = d * 3.14;
    float2 distortionOffset = float2(sin(rotOffset), cos(rotOffset)) * distortion;
 
    float2 distortedCoords = coords + distortionOffset;
    float2 offset = float2(t * -0.05, 0.0);
    float2 offset2 = float2(t * 0.05, 0.3);
    
    float4 sample1 = tex2D(SpriteSampler, distortedCoords + offset);
    float4 sample2 = tex2D(SpriteSampler, distortedCoords + offset2);
    float4 color = (sample1 + sample2) / 2.0;
    float4 finalColor = color;
    return finalColor;
}

float4 SampleFoam(float2 coords)
{
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float2 offsetCoords = (coords * tiling * 2.0) + float2(0.0, time * -0.05);
    offsetCoords += screenOffset * 4.0;
    
    float foam = tex2D(foamNoiseSampler, offsetCoords);
    float power = lerp(8.0, 0.3, heightMapColor.a);
    foam = pow(foam, power);
    float4 foamColor = float4(foam, foam, foam, 1.0);
    return foamColor * foam * 2.0;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    input.TextureCoordinates += screenOffset;
    input.TextureCoordinates = frac(input.TextureCoordinates);
    float4 baseColor = SampleSpriteNoise(input, SpriteTextureSampler) + SampleSpriteNoise(input, brightenNoiseSampler) * 0.5;
    
    float4 gradientColor = baseColor;
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);
    float3 gradient = lerp(endGradient, startGradient, heightMapColor.a);
    gradient *= gradient;
    gradientColor.rgb *= gradient;
    
    float4 caustics = SampleCausticsNoise(input, causticsNoiseSampler);
    
    float4 foam = SampleFoam(coords);
    foam *= foamLava;
    

    return gradientColor + caustics * causticsColor + foam;
}

float4 WrapPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    coords += screenOffset;
    coords = frac(coords);
    //Distort the coordinates
    float4 finalColor = tex2D(SpriteTextureSampler, coords);
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
    return finalColor * input.Color;
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
    offsetCoords += screenOffset;
    
    float foam = tex2D(SpriteTextureSampler, offsetCoords);
    float power = lerp(8.0, 0.3, heightMapColor.a);
    foam = pow(foam, power);
    float4 foamColor = float4(foam, foam, foam, 1.0);
    return foamColor * foam * 2.0;
}


//Based on https://www.shadertoy.com/view/4sdBRl
//Edited to hopefully be a bit simpler and more performance
float4 GetBlurCoords(float2 uv, float lod)
{
    float4 blurredCoords = float4(uv.x, uv.y, 0.0, lod);
    return blurredCoords;
}

float4 BlurPS(VertexShaderOutput input) : COLOR
{
    const float lod = 4.0;
    const float samples = 6.0;
    
    float2 coords = input.TextureCoordinates;

    float4 currentColor = tex2Dlod(SpriteTextureSampler, GetBlurCoords(coords, lod));
    float2 d = float2(0.0, 0.001);
    for (float i = 1.0; i < samples; i++)
    {
        currentColor += tex2Dlod(SpriteTextureSampler, GetBlurCoords(coords + d * i, lod));
        currentColor += tex2Dlod(SpriteTextureSampler, GetBlurCoords(coords - d * i, lod));

    }
    return currentColor / (samples * 2.0);
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
    float4 finalColor = fancyWaterColor * baseWaterColor.a * (1.0 - lavaMult) + baseWaterColor * lavaMult;
    return finalColor * input.Color;
}

float4 CombineALLPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 heightMapColor = tex2D(HeightMapTextureSampler, coords);     
    float3 gradient = lerp(endGradient, startGradient, heightMapColor.a);
    
    float4 baseWaterColor = tex2D(SpriteTextureSampler, coords);
    float4 fancyWaterColor = tex2D(WaterTextureSampler, coords);
    
        //sample outline
    if (baseWaterColor.a <= 0.0)
    {
        float2 leftCoords = coords + float2(-outlineTexelSize.x, 0.0);
        float2 rightCoords = coords + float2(outlineTexelSize.x, 0.0);
        float2 upCoords = coords + float2(0.0, -outlineTexelSize.y);
        float2 downCoords = coords + float2(0.0, outlineTexelSize.y);
   
        float4 left = tex2D(SpriteTextureSampler, leftCoords);
        float4 right = tex2D(SpriteTextureSampler, rightCoords);
        float4 up = tex2D(SpriteTextureSampler, upCoords);
        float4 down = tex2D(SpriteTextureSampler, downCoords);
    
        if (left.a > 0)
            return left.a * outlineColor;
        if (right.a > 0)
            return right.a * outlineColor;
        if (up.a > 0)
            return up.a * outlineColor;
        return down.a * outlineColor;
    }

    float4 finalColor = fancyWaterColor * baseWaterColor.a;
    return finalColor * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
technique WrapDrawing
{
    pass P1
    {
        PixelShader = compile PS_SHADERMODEL WrapPS();
    }
};
technique HeightDrawing
{
    pass P2
    {
        PixelShader = compile PS_SHADERMODEL HeightPS();
    }
};
technique ReflectionDrawing
{
    pass P3
    {
        PixelShader = compile PS_SHADERMODEL ReflectPS();
    }
};
technique GradientDrawing
{
    pass P4
    {
        PixelShader = compile PS_SHADERMODEL GradientPS();
    }
};

technique CausticsDrawing
{
    pass P5
    {
        PixelShader = compile PS_SHADERMODEL CausticsPS();
    }
};

technique SparklingCausticsDrawing
{
    pass P6
    {
        PixelShader = compile PS_SHADERMODEL SparklingCausticsPS();
    }
};

technique FoamDrawing
{
    pass P7
    {
        PixelShader = compile PS_SHADERMODEL FoamPS();
    }
};

technique PosterizeDrawing
{
    pass P8
    {
        PixelShader = compile PS_SHADERMODEL PosterizePS();
    }
};

technique BlurDrawing
{
    pass P9
    {
        PixelShader = compile PS_SHADERMODEL BlurPS();
    }
};

technique CombineRTDrawing
{
    pass P10
    {
        PixelShader = compile PS_SHADERMODEL CombinePS();
    }
};

technique CombineRTAllDrawing
{
    pass P11
    {
        PixelShader = compile PS_SHADERMODEL CombineALLPS();
    }
};
