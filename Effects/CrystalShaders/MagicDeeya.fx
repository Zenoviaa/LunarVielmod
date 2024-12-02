matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;

sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

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


const int MAXITER = 12;
float3 field(float3 p)
{
    p *= 0.1;
    float f = 0.1;
    for (int i = 0; i < 2; i++)
    {
        float3x3 mat = float3x3(0.8, 0.6, 0.0, -0.6, 0.8, 0.0, 0.0, 0.0, 1.0);
        p = mul(p.yzx, mat);
        p += float3(.123 * cos(time * 0.021) * float(i), .456 * sin(time * 0.021) * float(i), 0.789) * float(i);
        p = abs(frac(p) - 0.5);
        p *= 2.0;
        f *= 2.0;
    }
    p *= p;
    return sqrt(p + p.yzx) / f - .002;
}
#define H(h)(cos((h)*6.3+float3(0.0,23.0,21.0))*0.5+0.5)
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Calculate Distortion
    float4 O = input.Color;
    float2 uv = input.TextureCoordinates;
    float3 dir = normalize(float3((uv.xy - 0.5) / uv.x, 1.0));
    float3 pos = float3(1.0, 1.0, -sin(time * 2.0) * 5.0);
    float3 color = float3(0.0, 0.0, 0.0);
    for (int i = 0; i < MAXITER; i++)
    {
        float3 f2 = field(pos);
        float f = min(min(f2.x, f2.y), f2.z);
		
        pos += dir * f;
        color += float(2 - i) / (f2 + .001);
    }
    float3 color3 = float3(1.0 - 1.0 / (1.0 + color * (0.09 / float(MAXITER))));
    float3 p, q, r = float3(uv, 1.0),
    d = normalize(float3((uv * 2.0 - r.xy) / r.y, 1));
    for (float i = 0., a, s, e, g = 0.;
        ++i < 70.0;
        O.xyz += lerp(float3(1.0, 1.0, 1.0), H(g * 0.1), .8) * 10. / e / 8e3
    )
    {
        p = g * d * color3;
       
        a = 30.0;
        p = fmod(p - a, a * 2.0) - a;
        s = 2.0;
        for (int i = 0; i++ < 2;)
        {
            p = .5 - abs(p);
            p.x < p.z ? p = p.zyx : p;
            p.z < p.y ? p = p.xzy : p;
            s *= e = 1.7 + sin(time * .001) * .1;
            p = abs(p) * e -
                float3(
                    10. * 3.,
                    120,
                    5.0 * 5.0
                 );
        }
        g += e = length(p.yz) / s;
    }
    return O;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}