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

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    //Get the correct center point by fixing the y coordinate
    float yFixer = (uSourceRect.y + uSourceRect.w) / uImageSize0.y;
    float fixedY = coords.y / yFixer;
    
    float2 refCoords = float2(coords.x, fixedY);
    float2 diff = refCoords - float2(0.5, 0.5);
    float4 color = tex2D(uImage0, coords);
    float dist = length(diff) / 0.5;
    dist = saturate(dist);
    
    float fade = sin(dist * 3.14) * 0.5 + 0.5;
    color *= sampleColor;
    color *= fade * fade * fade;
    return color;
}

technique Technique1
{
	pass PixelPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}