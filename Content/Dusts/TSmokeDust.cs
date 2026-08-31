using Stellamod.Core;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts
{
    public class TSmokeDust : ModDust
    {

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 64 * Main.rand.Next(4), 64, 64);
            dust.rotation = Main.rand.NextFloat(-2f, 2f);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(ModContent.Request<Effect>("Stellamod/Effects/SmokeDust"), "PixelPass");
            dust.scale *= 1.5f;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale * 0.5f;
                dust.customData = true;
            }

            dust.position += dust.velocity;
            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.95f;

            dust.color *= 0.99f;
            dust.scale *= 0.99f;
            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
}
