sampler spriteSampler : register(s0);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float range = 0.01;
    float low = 0.45;
    float dist = length(coords - float2(0.5, 0.5));
    
    float osc = sin(dist * 3.14 + time + coords.y * 38.0) * 0.01;
    low += osc;
 
    if (dist > low && dist < low + range)
    {
        float a = dist - low;
        a /= range;
        float s = sin(a * 3.14);
        float4 aura = float4(1.0, 0.2 + osc * 4.0, 1.0, 1.0);
        return aura * s;
    }
    return float4(0.0, 0.0, 0.0, 0.0);

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}