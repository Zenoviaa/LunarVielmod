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


float time;
float frequency = 20.0;
float amplitude = 0.02;
float levels = 12.0;
float seaThreshold = 0.2;
float2 seaTiling;
float seaDarkness;
texture seaNoiseTexture;
sampler2D SeaNoiseSampler = sampler_state
{
    Texture = <seaNoiseTexture>;
    AddressU = wrap;
    AddressV = wrap;
};
float ringPower;
float3 ringColor;
float posterize(float v, float k)
{
    return ceil(v * k) / k;
}

float3 posterize(in float3 color, float factor)
{
    color.r = posterize(color.r, factor);
    color.g = posterize(color.g, factor);
    color.b = posterize(color.b, factor);
    return color;
}
float3 blackSea(float2 uv)
{
    uv *= seaTiling;
    uv.x = posterize(uv.x, 64.0);
    uv.y = posterize(uv.y, 64.0);
  
    //Basically we're going to make two scrolling textures, and the darkest parts will multiply with this effect
    //First generate the first scrolling texure
    float2 scrollingUv = uv;

    scrollingUv += float2(time * 0.05, 0.01);
    float3 seaColor1 = tex2D(SeaNoiseSampler, scrollingUv).rgb;
    
    float2 scrollingUv2 = uv;
    scrollingUv2 += float2(time * -0.05, time * -0.02);
    float3 seaColor2 = tex2D(SeaNoiseSampler, scrollingUv2).rgb;
    
    float3 seaColor = lerp(seaColor1, seaColor2, 0.5);
    float avg = seaColor.r + seaColor.g + seaColor.b;
    avg /= 3.0;
    
    float3 blackSeaColor = float3(avg, avg, avg);
    blackSeaColor = posterize(blackSeaColor, 4.0);
    blackSeaColor = lerp(blackSeaColor, float3(0.0, 0.0, 0.0), seaDarkness);
    return blackSeaColor;
}
float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float3 ring(float2 uv)
{
    //uv *= 0.75;
    uv.y += sin(time + uv.x * 20.0) * 0.1;
   // uv.y += iTime * 0.05;
    float maxDistance = length(float2(0.0, 0.0) - float2(0.5, 0.5));
    float dist = length(uv - float2(0.5, 0.5));
    float lerp = dist / maxDistance;
    float p = pow(lerp, ringPower);
    return float3(p, p, p) * ringColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;
    uv.y += sin(time * frequency * uv.x) * amplitude;
    //So first sample the texture color
    float3 col = tex2D(uImage0, uv).rgb;
    float avg = col.r + col.g + col.b;
    avg /= 3.0;
    col = float3(avg, avg, avg);
    
    
    //The first thing we want to do is determine if the color is bright or dark
    //Dark colors go darker and light colors go lighter
    //Calculate the brightness with this formula
    float lums = 0.2126 * col.r + 0.7152 * col.g + 0.0722 * col.b;
    float whiteBend = uColor.r;
    float blackBend = uColor.g;
    float lightThreshold = uColor.b;
    if (lums > lightThreshold)
    {
        col = lerp(col, float3(1.0, 1.0, 1.0), whiteBend);
    }
    else
    {
        col = lerp(col, float3(0.0, 0.0, 0.0), blackBend);
    }
    
    //Apply some posterization to limit the palette
    col = posterize(col, levels);

    float3 seaColor = blackSea(uv);
    if (col.r < seaThreshold)
    {
        col = lerp(col, seaColor, 0.5);
    }
  
    col += ring(coords);
    float4 fragColor = float4(col, 1.0);
    return fragColor;
}

technique SpriteDrawing
{
    pass ScreenPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};