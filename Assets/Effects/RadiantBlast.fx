sampler baseTarget : register(s0);
sampler noiseTexture : register(s1);
#define PI 3.14159

float Time;
float4 InnerColor;
float4 GlowColor;
float4 TrailColor;
float NoiseScale = 1.0;
float Distortion = 1.0;
float2 CurveDirection;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //Alright this is the part I've been waiting for
    //We gotta make the shader in the image
    
    //Need to sample the noise texture first.
    coords += lerp(float2(0.0, 0.0), CurveDirection, coords.x * coords.x);
    
    
    
    float2 noiseCoords = coords + float2(0.005f, 0.0f) * Time;
    float noiseSample = tex2D(noiseTexture, noiseCoords);
    float rot = lerp(0, PI, noiseSample);
    float2 angleOffset = float2(sin(rot), cos(rot));
    angleOffset *= Distortion;
    
    //Now we have the offset to the coorsd
    //So
    float2 shapeCoords = coords + angleOffset;
    float4 coreColor = tex2D(baseTarget, shapeCoords);
   
    float2 glowingCoords = coords + angleOffset * 0.5f;
    float4 glowingColor = tex2D(baseTarget, glowingCoords);
    
    
    //Lerp to the trail color
    float4 glowColor = lerp(TrailColor, GlowColor, coords.x);
    glowingColor *= lerp(InnerColor, glowColor, noiseSample);
    return (glowingColor + coreColor) * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}