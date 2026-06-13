sampler glowMaskSampler : register(s0);
sampler noiseSampler : register(s1);
matrix transformMatrix;
float3 flameStartColor;
float3 flameBloomColor;
float time;
float distortion;

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


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 tintColor = input.Color;
    float2 coords = input.TextureCoordinates;
    //Apply distortion to the blowtorch so it doesn't just look like one straight circle lol
    float2 noiseCoords = coords + float2(time * 0.05, 0.0);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(noiseSampler, noiseCoords).r;
    float2 distortionOffset = float2(0.0, sin(n * 1.2 * 8.0)) * lerp(distortion * sin(time + coords.x * 4.0), 0.0, coords.x);
    //distortionOffset = round(distortionOffset * 12.0) / 12.0;
    float2 sampleCoords = coords + distortionOffset;
    sampleCoords += float2(0.0, sin(time + coords.x * 8.0) * 0.015);
    
    //Applying the same technique again, we scroll a 
    //This time we're going to actually apply a gradient to the glow so it isn't the asme glow from the start to the end
    //This will genuinely just look better
    //I think we go from yellow to red
    float4 glowMask = tex2D(glowMaskSampler, sampleCoords);
    float3 flameColor = lerp(flameStartColor, flameBloomColor, sampleCoords.x);

    glowMask.rgb *= flameColor;
    glowMask.rgb *= lerp(1.5, 0.0, coords.x) * glowMask.r * 8.0;
    
    
    float2 diff = (sampleCoords - float2(0.5, 0.5));
    float len = length(diff);
    float interp = saturate(len / 0.5);
    interp = 1.0 - interp;
    interp = pow(interp, 3.0);
    glowMask.rgb += interp;
   
    float d = n < coords.x;
    float d2 = max(glowMask.r, max(glowMask.g, glowMask.b)) < 0.12;
    d2 = 1.0 - d2;
    glowMask *= d;
    glowMask *= d2;
    glowMask = round(glowMask * 12.0) / 12.0;
    return glowMask * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}