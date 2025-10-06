sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float2 uTargetPosition;

float4 StartColor;
float4 GlowColor;

texture NoiseTexture;
sampler2D NoiseSampler = sampler_state
{
    texture = <NoiseTexture>;
    magfilter = POINT;
    minfilter = POINT;
    mipfilter = POINT;
    AddressU = mirror;
    AddressV = mirror;
};

float2 Tiling = float2(1.0, 1.0);
float2 Offset = float2(0.0, 0.0);
#define PI 3.14159

float QuadraticBump(float time)
{
    return time * (4.0 - time * 4.0);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{		
	//Convert to polar coordinates and get the distance from the center
	//Plug that into quadratic bump
    float2 uv = coords;
    //Convert to polar coordinates and scroll outward
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;

    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    float interpolant = QuadraticBump(dist);
    	
    //Scroll a noise over this color
    float2 noiseUv = coords;
    noiseUv *= Tiling;
    noiseUv += Offset;
    
    //Sample the image 
    float noise = tex2D(NoiseSampler, noiseUv);
    
	//Lerp between the start and glow colors
    //Multiply the interpolant by the noise to add like distortion/variation in it
    float4 color = lerp(StartColor, GlowColor, interpolant * noise) * interpolant * 0.5f;
    float4 baseColor = tex2D(uImage0, coords);
    return baseColor + color;
}

technique Technique1
{
	pass GlowingDustPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}