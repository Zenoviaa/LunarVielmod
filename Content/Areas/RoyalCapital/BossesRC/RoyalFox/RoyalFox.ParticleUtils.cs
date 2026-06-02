using Stellamod.Assets;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{
    private float _stepDistance;

    public static void PlayAirbounceSuond(Vector2 position)
    {
        int soundIndex = Main.rand.Next(2);
        SoundStyle airbounce;
        switch (soundIndex)
        {
            default:
            case 0:
                airbounce = AssetRegistry.Sounds.AlcaricFox.FenixAirbounce1;
                break;
            case 1:
                airbounce = AssetRegistry.Sounds.AlcaricFox.FenixAirbounce2;
                break;
        }
        SoundEngine.PlaySound(airbounce, position);
    }
    public static void CreateRoyalStarSmoke(Vector2 position, Vector2 velocity)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
        royalMagicRenderer.SpawnParticle(position, velocity, 180);
    }
    public static void CreateRoyalStarSmallSmoke(Vector2 position, Vector2 velocity)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        RoyalMagicRenderer royalMagicRenderer = ModContent.GetInstance<RoyalMagicRenderer>();
        royalMagicRenderer.SpawnParticle(position, velocity, 130);
    }
    public static void PlayImpactSound(Vector2 position)
    {
        int soundIndex = Main.rand.Next(4);
        SoundStyle dashSound;
        switch (soundIndex)
        {
            default:
            case 0:
                dashSound = AssetRegistry.Sounds.AlcaricFox.Fenixsmallcrash1;
                break;
            case 1:
                dashSound = AssetRegistry.Sounds.AlcaricFox.Fenixsmallcrash2;
                break;
            case 2:
                dashSound = AssetRegistry.Sounds.AlcaricFox.Fenixsmallcrash3;
                break;
            case 3:
                dashSound = AssetRegistry.Sounds.AlcaricFox.Fenixsmallcrash4;
                break;
        }
        SoundEngine.PlaySound(dashSound, position);
    }
    public static void PlayDashSound(Vector2 position)
    {
        int soundIndex = Main.rand.Next(4);
        SoundStyle dashSound;
        switch (soundIndex)
        {
            default:
            case 0:
                dashSound = AssetRegistry.Sounds.AlcaricFox.FenixFastdash1;
                break;
            case 1:
                dashSound = AssetRegistry.Sounds.AlcaricFox.FenixFastdash2;
                break;
            case 2:
                dashSound = AssetRegistry.Sounds.AlcaricFox.FenixFastdash3;
                break;
            case 3:
                dashSound = AssetRegistry.Sounds.AlcaricFox.FenixFastdash4;
                break;
        }
        SoundEngine.PlaySound(dashSound, position);
    }
    public void CreateFootsteps()
    {
        float traveledDistance = Vector2.Distance(NPC.position, NPC.oldPosition);
        _stepDistance += traveledDistance;
        if (_stepDistance >= 100)
        {
            Vector2 pos = NPC.Bottom + new Vector2(Main.rand.NextFloat(-32f, 32f), 0);
            pos.Y += 24;
            var circleStep = LegacyParticle.NewParticle<CircleStepParticle>(pos, Vector2.UnitY);
            circleStep.color = Color.Blue;
            circleStep.Rotation = NPC.rotation;
            _stepDistance = 0;
        }
    }
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
    public static void ChargeParticlesBig(Vector2 center, in float timer)
    {
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            //   fx.VectorScale *= 0.5f;
        }
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            //   fx.VectorScale *= 0.5f;
        }

        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.Scale *= 0.35f;
            dp.outerColor = Color.Violet;
            dp.gravity = 0;
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
    public void TeleportEffect(Vector2 position)
    {
        PoofParticles(NPC.Center);
        float dir = Main.rand.NextBool(2) ? 1 : -1;
        Teleport(position);
        PoofParticles(position);

        ShakeScreenPosition.Shake = 8;
        var fx = FXUtil.GlowCircleBoom(position, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 30, baseSize: 0.2f); ;
        fx.Scale *= 2f;
        for (int i = 0; i < 32; i++)
        {
            var dp = DustParticle.Spawn(position, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.DarkBlue;
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.Scale *= 1.5f;
        }

        PixelPrimitiveCircleFactory.CreateGenericInBoom(position, Color.White, Color.Transparent, 60, 512);
        PixelPrimitiveCircleFactory.CreateGenericBoom(position, Color.White, Color.Transparent, 60, 512);
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
    private void FloatParticles()
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
            var sp = FaintSmokeParticle.SpawnInAlphaLayer(NPC.Center + Main.rand.NextVector2Circular(64, 64), -vel, Scale: Main.rand.NextFloat(0.25f, 0.6f));
            sp.Scale *= 0.5f;
            sp.color = Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat(0f, 0.33f));
            sp.behindLayer = true;
        }
    }

}
