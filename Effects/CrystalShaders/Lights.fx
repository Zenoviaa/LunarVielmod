sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

matrix worldViewProjection;
float2 stepSize;
texture geometryTexture;

sampler2D geometrySampler = sampler_state
{
    texture = <geometryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float2 ScreenCenterCoordinates : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float2 ScreenCenterCoordinates : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, worldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.ScreenCenterCoordinates = input.ScreenCenterCoordinates;
    return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 lightColor = input.Color.rgb;
    float lightIntensity = input.Color.a;
    float4 diffuseLight = float4(0.0, 0.0, 0.0, 1.0);
   
    float2 baseUV = input.TextureCoordinates;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float attenuation = lerp(1.0, 0.0, saturate(dist));
    
    
    
    //Ray casting for shadows
    float2 moveVector = input.TextureCoordinates - float2(0.5, 0.5);
  
    moveVector = normalize(moveVector);
    float2 origin = input.ScreenCenterCoordinates;
    float strength = 1.0;
    float maxSteps = dist / length(stepSize);
    for (float f = 0.0; f < 32.0; f++)
    {
        float2 currentScreenCoordinates = origin + moveVector * stepSize * f;
        float solid = tex2D(geometrySampler, currentScreenCoordinates).a;
        float falloff = solid * 0.5;
        strength -= falloff;
        if(strength <= 0.0 || f > maxSteps)
            break;
    }
    
    
    diffuseLight.rgb += lightColor * attenuation * strength;
    return diffuseLight;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}