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
float3 vortexDarkColor;
float3 vortexLightColor;

float3 centerColor;
float3 outerColor;

float2 rotate(float2 uv, float2 pivot, float angle)
{
    //rotation matrix
    float2x2 rotation = float2x2(
            float2(sin(angle), -cos(angle)),
			float2(cos(angle), sin(angle)));
    
    uv -= pivot;
    uv = mul(uv, rotation);
    uv += pivot;
    return uv;
}

float3 getVortexColor(float n)
{
    return lerp(vortexDarkColor, vortexLightColor, n);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    //First we multiply the coordinates by the distance from the center to get a 
    //Distorting effect
    float dist = length((uv - float2(0.5, 0.5)));
    uv *= dist * 2.0;
    
    //Then we're going to take the two textures and rotate them in opposite directions
    //This should create some nice variation in the texture
    float speed = 1.0;
    float2 clockwise = rotate(uv, float2(0.5, 0.5), time);
    float2 cclockwise = rotate(uv, float2(0.5, 0.5), time);
    
    //Calculate a smoothing gradient from the center to the outside
    float3 gradient = lerp(centerColor, outerColor, dist * dist);
    float4 col = tex2D(uImage0, clockwise);
    
    //Calculate a vortex color
    float3 vColor1 = getVortexColor(col.r);
    col.rgb = col.rgb * gradient;
    
    float4 col2 = tex2D(uImage0, cclockwise);
    float3 vColor2 = getVortexColor(col.r);
    col2.rgb = col2.rgb * gradient;
    float4 finalCol = (col + col2) / 2.0;
    return finalCol * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};