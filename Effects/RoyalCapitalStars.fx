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

texture primaryTexture;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float2 primaryTextureSize;
float2 resolution;

// 3D Gradient noise from: https://www.shadertoy.com/view/Xsl3Dl
float3 hash(float3 p) // replace this by something better
{
    const float3 a = float3(127.1, 311.7, 74.7);
    const float3 b = float3(269.5, 183.3, 246.1);
    const float3 c = float3(113.5, 271.9, 124.6);
    
    
    
    p = float3(dot(p, a),
			  dot(p, b),
			  dot(p, c));

    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

float noise(in float3 p)
{
    float3 i = floor(p);
    float3 f = frac(p);
	
    float3 u = f * f * (3.0 - 2.0 * f);

    return lerp(lerp(lerp(dot(hash(i + float3(0.0, 0.0, 0.0)), f - float3(0.0, 0.0, 0.0)),
                          dot(hash(i + float3(1.0, 0.0, 0.0)), f - float3(1.0, 0.0, 0.0)), u.x),
                     lerp(dot(hash(i + float3(0.0, 1.0, 0.0)), f - float3(0.0, 1.0, 0.0)),
                          dot(hash(i + float3(1.0, 1.0, 0.0)), f - float3(1.0, 1.0, 0.0)), u.x), u.y),
                lerp(lerp(dot(hash(i + float3(0.0, 0.0, 1.0)), f - float3(0.0, 0.0, 1.0)),
                          dot(hash(i + float3(1.0, 0.0, 1.0)), f - float3(1.0, 0.0, 1.0)), u.x),
                     lerp(dot(hash(i + float3(0.0, 1.0, 1.0)), f - float3(0.0, 1.0, 1.0)),
                          dot(hash(i + float3(1.0, 1.0, 1.0)), f - float3(1.0, 1.0, 1.0)), u.x), u.y), u.z);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    const float stars_threshold = 10.0f; // modifies the number of stars that are visible
    const float stars_exposure = 200.0f; // modifies the overall strength of the stars

    // Stars computation:

    float3 stars_direction = normalize(float3((coords * 2.0f - 1.0f), 1.0f)); // could be view vector for example
    stars_direction.xy += uImageOffset * 0.02;
    
    float stars = pow(clamp(noise(stars_direction * 200.0f), 0.0f, 1.0f), stars_threshold) * stars_exposure;
    stars *= lerp(0.4, 1.4, noise(stars_direction * 100.0f + float3(uTime, uTime, uTime))); // time based flickering
	
    // Output to screen
    float4 fragColor = float4(stars, stars, stars, 0.0);
    return fragColor * uOpacity;
    /*
    float2 starNoiseCoords = (coords * resolution - uSourceRect.xy) / primaryTextureSize;
    float starNoise = tex2D(primaryTex, starNoiseCoords + uImageOffset).r;
    
    float c = length(coords);
    starNoise *= sin(c * uTime * 0.05f);
    
    float distortingNoise = tex2D(uImage2, coords + float2(uTime * -0.003, -0.0015) + uImageOffset).r;
    starNoise *= distortingNoise;
    
    float4 finalColor = float4(starNoise, starNoise, starNoise, 0.0);
    finalColor *= uOpacity;
    return finalColor;*/
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};