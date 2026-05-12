sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

#define PI 3.1415926535897931;
float time;
float frequency;
float amplitude;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
 
    //Calculate a wave based on the angle from the center
    float2 uv = coords;
    float2 diff = uv - float2(0.5, 0.5);
    float radians = atan2(diff.y, diff.x);
    float interp = radians / PI;
    
    float f = interp * frequency * PI;
    float osc = sin(f + time);
       
    float l = length(diff);
    float2 normalDifference = diff / l;
    float2 distortionOffset = normalDifference * osc * amplitude;
    
    float n = tex2D(uImage0, uv + distortionOffset).r;
    
    //Just a little bit of bloom
    n = pow(n, 0.5);
    
    //Mixed in the sample palette gradient 
    float2 gradientCoords = float2(0.0, n);
    float4 gradient = tex2D(uImage1, gradientCoords);
    float4 mixedColor = gradient * sampleColor * n;
    return mixedColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};