using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{
    public static void SpawnCometStarParticle(Vector2 center, Vector2 velocity, float timeLeft)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicCometStarsRenderer starsRender = ModContent.GetInstance<RoyalMagicCometStarsRenderer>();
        starsRender.SpawnParticle(center, velocity, timeLeft);
    }

    public static void ChargeParticles(Vector2 center, in float timer)
    {
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(252, 252);
            Vector2 vel = (center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }

        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(252, 252);
            Vector2 vel = (center - pos);
            vel *= 0.15f;
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.Scale *= 0.35f;
            dp.outerColor = Color.Violet;
        }
    }

    private void PoofParticles()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
        for (float f = 0; f < 13; f++)
        {

            Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(3f, 7f);
            royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 128), vel, 180);

            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            }
            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                sp.behindLayer = Main.rand.NextBool(2);
            }
            if (Main.rand.NextBool(2))
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.Scale *= 0.5f;
                sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
                sp.behindLayer = true;
            }
        }
    }
    public static void PoofParticles(Vector2 centerPos)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
        for (float f = 0; f < 13; f++)
        {

            Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(3f, 7f);
            royalMagicRenderer.SpawnParticle(centerPos + Main.rand.NextVector2Circular(64, 128), vel, 180);

            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicStarParticle.Spawn(centerPos + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            }
            if (Main.rand.NextBool(2))
            {
                var sp = RoyalMagicSwordParticle.Spawn(centerPos + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                sp.behindLayer = Main.rand.NextBool(2);
            }
            if (Main.rand.NextBool(2))
            {
                var sp = FaintSmokeParticle.SpawnInAlphaLayer(centerPos + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
                sp.Scale *= 0.5f;
                sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
                sp.behindLayer = true;
            }
        }
    }
    private void WalkParticles()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();


        if (Main.rand.NextBool(4))
        {
            var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.GemDiamond, Scale: 1f);
            d.noGravity = true;
        }

        if (!Main.rand.NextBool(2))
            return;


        Vector2 vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7f);
        royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, 180);

        vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7f);
        royalMagicRenderer.SpawnParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, 180);

        if (!Main.rand.NextBool(4))
            return;

        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
        }
        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            sp.behindLayer = Main.rand.NextBool(2);
        }
        if (Main.rand.NextBool(2))
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.Scale *= 0.5f;
            sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
            sp.behindLayer = true;
        }
    }
    private void WalkParticles2()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();


        if (Main.rand.NextBool(4))
        {
            var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(16, 16), DustID.GemDiamond, Scale: 1f);
            d.noGravity = true;
        }

        if (!Main.rand.NextBool(2))
            return;


        if (!Main.rand.NextBool(4))
            return;

        Vector2 vel = -NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f);
        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
        }
        if (Main.rand.NextBool(2))
        {
            var sp = RoyalMagicSwordParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
            sp.behindLayer = Main.rand.NextBool(2);
        }
        if (Main.rand.NextBool(2))
        {
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.Scale *= 0.5f;
            sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
            sp.behindLayer = true;
        }
    }

}
