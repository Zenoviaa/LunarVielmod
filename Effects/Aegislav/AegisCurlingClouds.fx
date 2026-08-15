sampler buffer : register(s0);
sampler convectionNormalSampler : register(s1);
sampler convectionMaskSampler : register(s2);
sampler swirlNormalSampler : register(s3);
float time;
float2 res;
float firstFrame;
float2 cameraMovement;

//Adapted and learning from
//http://petewerner.blogspot.com/2015/02/intro-to-curl-noise.html
//ref https://www.shadertoy.com/view/cl23Wt

//gotta see what a gyroid is
float gyroid(float3 p)
{
    return dot(sin(p), cos(p.yzx));
}

//TODO: experiment with replacing this part entirely with a normal map texture read
//That would be a lot more customizable
float MovementNoise(float3 p)
{
    float result = 0., a = .5;
    float count = res.y < 500. ? 6. : 8.;
    for (float i = 0.; i < count; ++i, a /= 2.)
    {
        p.z += time * .02; //+result*.5;
        result += abs(gyroid(p / a)) * a;
    }
    return result;
}

float3 ConvectionCurrent(float2 coords)
{
    //Have the normal coordinates between -1 and 1 instead of 0-1
    float3 normalVec = tex2D(convectionNormalSampler, coords).rgb * 2.0 - 1.0;
    normalVec.x *= -1;
    return normalVec;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //this render target does not clear its contents
    //So we're basically going to make a particle sim!
    float3 color = float3(0.0, 0.0, 0.0);
    
    float2 uv = coords;
    float2 fragCoord = uv * res;
    float2 offset = float2(0.0, 0.0);
    float2 e = float2(.01, 0.0);
    
    float mask = tex2D(convectionMaskSampler, coords).r;
    
    
    float2 p = (2. * fragCoord - res.xy) / res.y;
    float3 pos = float3(p, length(p) * .5);

    //Seems like this just mixes together noise offsets to create a single blended noise
    //I'm going to try replacing this part entirely
    //The main part of this is the fact that it's basically just simulating particles.
    //So we should be able to replace it with pretty much any type of ovement
    float x = (MovementNoise(pos + e.yxy) - MovementNoise(pos - e.yxy)) / (2. * e.x);
    float y = (MovementNoise(pos + e.xyy) - MovementNoise(pos - e.xyy)) / (2. * e.x);
    float3 convection = ConvectionCurrent(coords);
    float2 curl = float2(x, -y);

    // force fields
    offset += curl;
   // offset.y += coords.y * 5.0;
   // offset.x -= coords.x * 4.0;
    //Generaly movement current from the normal map
    offset += convection.xy * 2.2;
    offset += cameraMovement * 0.05;
    // displace buffer sampler coordinates
    uv += offset * .0004 * float2(res.y / res.x, 1);
  //  uv += convection.xy * 0.0005;
    float3 frame = tex2D(buffer, uv).rgb;
    

    // spawn from edge
    bool spawn = fragCoord.x < 1.0 || fragCoord.x > res.x - 1.
        || fragCoord.y < 1.0 || fragCoord.y > res.y - 1.;
    
    // spawn at first frame
    spawn = spawn || firstFrame < 1;
    if (spawn)
    {
        color = .5 + .5 * cos(float3(1.0, 2.0, 3.0) * 5.5 + time * 0.03 + (uv.x + uv.y) * 6.);

        // color.xyz = sin(time * 0.03 + (uv.x + uv.y) * 6.0) * 0.5 + 0.5;
        color *= sampleColor;
    }
    else
    {
        //If we're not spawning particles at the edge then take the brightest color at this spot
        color = max(color, frame);
    
    }
     
//    color *= lerp(0.85, 1.0, mask);
    return float4(color, 1.0);
 }

technique Technique1
{
    pass ParticleSimPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}