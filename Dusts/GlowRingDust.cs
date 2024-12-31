using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace Stellamod.Dusts
{
    public class GlowRingDust : ModDust
    {

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 53, 53);

            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(Stellamod.Instance.Assets.Request<Effect>("Effects/GlowingDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData is null)
            {
                dust.position -= Vector2.One * 32 * dust.scale;
                dust.customData = true;
            }

            Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

            dust.scale *= 0.95f;
            Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            dust.position += currentCenter - nextCenter;

            dust.shader.UseColor(dust.color);

            dust.position += dust.velocity;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;

            dust.velocity *= 0.97f;
            dust.color *= 0.97f;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());

            if (dust.scale < 0.05f)
                dust.active = false;

            return false;
        }
    }
}