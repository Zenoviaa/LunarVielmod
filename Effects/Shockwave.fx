float4 Shockwave(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    float dotField = dot(centreCoords, centreCoords);
    float ripple = dotField * uColor.y * PI - uProgress * uColor.z;

    if (ripple < 0 && ripple > uColor.x * -2 * PI)
    {
        ripple = saturate(sin(ripple));
    }
    else
    {
        ripple = 0;
    }

    float2 sampleCoords = coords + ((ripple * uOpacity / uScreenResolution) * centreCoords);

    return tex2D(uImage0, sampleCoords);
}