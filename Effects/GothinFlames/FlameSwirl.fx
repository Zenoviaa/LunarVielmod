sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float angleCenter;
float angleRadius;
float3 flameInsideColor;
float3 flameBloomColor;
float time;

float2 Rotate(float2 uv, float2 pivot, float angle)
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

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
       
    float2 diff = coords - float2(0.5, 0.5); 
    float len = length(diff);
    float distanceInterpolant = len / 0.5;
    distanceInterpolant = clamp(distanceInterpolant, 0.0, 1.0);
    
    coords = Rotate(coords, float2(0.5, 0.5), time * -0.2 + distanceInterpolant * 3.14);

    // Time varying pixel color
    float2 sampleCoords = coords + float2(time * -0.05, time * 0.05);
    sampleCoords = frac(sampleCoords);
    float3 col = tex2D(uImage0, sampleCoords).rgb; //(texture(iChannel0, uv).r);
    
    float2 sampleCoords2 = coords + float2(time * 0.05, time * -0.05);
    sampleCoords2 = frac(sampleCoords2 * 0.3);
    float3 col2 = tex2D(uImage0, sampleCoords2).rgb;
    
    float2 sampleCoords3 = coords + float2(time * -0.05, time * -0.05);
    sampleCoords3 = frac(sampleCoords3 * 4.0);
    float3 col3 = tex2D(uImage0, sampleCoords3).rgb;
    
    col += col2;
    col += col3 * sin(time * -0.2 + coords.x * 4.0);
    col *= 0.6;

    float angle = atan2(diff.y, diff.x);
    float angleDiff = abs(angleCenter - angle); //???
    float angleFade = saturate(angleDiff / angleRadius);
    float angleCutoff = angleDiff > angleRadius;

    float edgeFade = smoothstep(1.0, 0.0, distanceInterpolant);

    col *= lerp(flameInsideColor, flameBloomColor, distanceInterpolant);
    col += smoothstep(0.0, 0.4, distanceInterpolant) * col.r;
    col *= sin(edgeFade * 6.28) * 1.4;
    col *= pow(angleFade, 5.0);
    
    col = round(col * 4.0) / 4.0;
    float4 finalColor = float4(col, 1.0);
    return finalColor * sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}