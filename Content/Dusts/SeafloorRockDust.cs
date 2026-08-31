using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Content.Dusts;

public class SeafloorRockDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, Main.rand.Next(4) * 22, 22, 22);
        dust.color = Color.White;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return dust.color;
    }

    public override bool Update(Dust dust)
    {
        float halfSize = 22 / 2f;
        if (dust.customData is null)
        {
            dust.position -= Vector2.One * halfSize * dust.scale;
            dust.customData = true;
        }

        Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * halfSize * dust.scale;

        dust.scale *= 0.95f;
        Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * halfSize * dust.scale;

        dust.rotation += 0.06f;
        dust.position += currentCenter - nextCenter;

        dust.position += dust.velocity;

        if (!dust.noGravity)
            dust.velocity.Y += 0.1f;


        dust.velocity *= 0.95f;
        dust.color *= 0.99f;

        if (!dust.noLight)
            Lighting.AddLight(dust.position, dust.color.ToVector3());

        if (dust.scale < 0.05f)
            dust.active = false;

        return false;
    }
}