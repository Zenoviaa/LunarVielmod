using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;
using Stellamod.Effects.RoyalMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Stellamod.Common.DrawEffects;

public static class CommonDrawEffects
{
    public static void DrawFenixLaserCircles(SpriteBatch sb, in float timer, in Vector2 position, in Vector2 velocity, in float num)
    {
        float time = 25;
        StarBombBoomShader shockwave = ShaderContent.GetInstance<StarBombBoomShader>();
        shockwave.Time = MathHelper.Lerp(0f, 0.5f, EasingFunction.InExpo(timer / time));
        using (new SpritebatchContext(sb, SpritebatchParams.InWorldAndZoomed() with { effect = shockwave.Effect}))
        {
            for (int i = 0; i < num; i++)
            {
                float offset = 192;
                float between = 128;
                Vector2 offse2t = velocity.SafeNormalize(Vector2.Zero) * offset;
                Vector2 pos = position + offse2t + velocity.SafeNormalize(Vector2.Zero) * between * i;

                float scale = MathHelper.Lerp(1f, 0.2f, (float)i / 4f);
                SpritebatchDrawer circleDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<StarBombLaserShockwave>()], pos);
                float yScale = MathHelper.Lerp(0.2f, 2.3f, EasingFunction.OutExpo(timer / time)) * scale * 0.7f;
                circleDrawer.scale.Y *= yScale;
                circleDrawer.scale *= 0.75f;
                circleDrawer.rotation = velocity.ToRotation();

                Color color = Color.Lerp(Color.Blue, Color.Pink, scale);
                color = Color.Lerp(color, Color.Pink, EasingFunction.OutExpo(timer / (time / 2f)));

                circleDrawer.color = color;
                Main.spriteBatch.Draw(circleDrawer);
            }
        }
    }
}
