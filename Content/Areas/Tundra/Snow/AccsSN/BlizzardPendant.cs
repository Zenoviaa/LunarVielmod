using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.Generic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN;

public class BlizzardPendant : AbstractMeleeAddon
{
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        foreach (NPC npc in NPCHelper.FindNPCsInRange(projectile.Projectile.Center, 100))
        {
            npc.AddBuff(ModContent.BuffType<BlizzardChill>(), 90);
        }
    }

    public override void OnHitNPC(BaseSwingProjectileV2 projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        if (projectile.IsFinishingSwing())
        {
            int rand = Main.rand.Next(0, 3);
            SoundStyle shootSound;// = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
            switch (rand)
            {
                default:
                case 0:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot1;
                    break;
                case 1:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot2;
                    break;
                case 2:
                    shootSound = AssetRegistry.Sounds.Illuria.SlushShot3;
                    break;
            }

            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.5f;
            SoundEngine.PlaySound(shootSound, target.position);

            target.AddBuff(ModContent.BuffType<BlizzardFreeze>(), 60);
        }
    }

    public override void PreDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor)
    {
        base.PreDrawEffects(projectile, ref lightColor);
        float a = EasingFunction.QuadraticBump(projectile.Interpolant);
        SpritebatchDrawer backGlow = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, projectile.Projectile.Center);
        backGlow.color = Color.LightSkyBlue * ExtraMath.Osc(0.5f, 1f, speed: 6) * 0.5f * a;
        backGlow.color.A = 0;
        backGlow.scale *= 0.5f;
        Main.spriteBatch.Draw(backGlow);
    }

    public override void PostDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor)
    {
        base.PostDrawEffects(projectile, ref lightColor);
        float a = EasingFunction.QuadraticBump(projectile.Interpolant);
        AuraShader blizzardShader = ShaderContent.GetInstance<AuraShader>();
        blizzardShader.Time = Main.GlobalTimeWrappedHourly * 8;
        SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed() with { effect = blizzardShader };
        using (SpritebatchStarter.Begin(Main.spriteBatch, spritebatchParams))
        {
            Asset<Texture2D> blizzardNoise = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/StarNoise");
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(blizzardNoise, projectile.Projectile.Center);
            drawer.color = Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 8)) * 0.4f * a;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }

        SpritebatchDrawer vortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, projectile.Projectile.Center);
        vortexDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        vortexDrawer.color = Color.White * 0.2f * a;
        vortexDrawer.color.A = 0;
        Main.spriteBatch.Draw(vortexDrawer);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankAccessory>();
    }
}

public class BlizzardFreezeGlobalNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.boss)
            return base.PreAI(npc);
        if (npc.HasBuff<BlizzardFreeze>())
            return false;
        return base.PreAI(npc);
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        Asset<Texture2D> crystalTexture = ModContent.Request<Texture2D>(ModContent.GetInstance<BlizzardFreeze>().Texture + "_Crystal");
        if (npc.HasBuff<BlizzardFreeze>())
        {
            int timeLeft = npc.buffTime[npc.FindBuffIndex(ModContent.BuffType<BlizzardFreeze>())];
            float maxTime = 60f;
            float interpolant = (float)timeLeft / maxTime;
            float ease = EasingFunction.InOutSine(interpolant);
            SpritebatchDrawer crystalDrawer = SpritebatchDrawer.FromTextureAsset(crystalTexture, npc.Center);
            crystalDrawer.scale = Vector2.One;
            crystalDrawer.color = Color.Lerp(Color.White * 0.5f, Color.White, ease);
            spriteBatch.Draw(crystalDrawer);

            crystalDrawer.color = Color.White * ExtraMath.Osc(0.25f, 0.6f, speed: 32);
            crystalDrawer.color.A = 0;
            spriteBatch.Draw(crystalDrawer);

            if(timeLeft == 25)
            {
                int rand = Main.rand.Next(0, 2);
                SoundStyle soundStyle;
                switch (rand)
                {
                    default:
                    case 0:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact1;
                        break;
                    case 1:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact2;
                        break;
                }
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, npc.position);

            }

            if (timeLeft == 2)
            {
                int rand = Main.rand.Next(0, 2);
                SoundStyle soundStyle;
                switch (rand)
                {
                    default:
                    case 0:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact1;
                        break;
                    case 1:
                        soundStyle = AssetRegistry.Sounds.Illuria.IceImpact2;
                        break;
                }
                soundStyle.PitchVariance = 0.3f;
                SoundEngine.PlaySound(soundStyle, npc.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 initialVelocity = Main.rand.NextVector2Circular(1, 1);
                    initialVelocity *= 6;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle<DustParticle>.Spawn(npc.Center, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.2f, 0.5f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }


                for (float f = 0; f < 2; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);


                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(npc.Center + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.14f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }
            }
        }
    }
}

public class BlizzardFreeze : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 8;

        float slowdownMult = 0f;
        npc.velocity *= slowdownMult;
    }
}

public class BlizzardChill : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 8;

        float slowdownMult = npc.boss ? 0.995f : 0.96f;
        npc.velocity *= slowdownMult;

        if (Main.rand.NextBool(5))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.DarkSlateBlue, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.LightSkyBlue, Color.DarkSlateBlue, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }

        if (Main.rand.NextBool(5))
        {
            EmberParticle e = LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.DarkSlateBlue, Main.rand.NextFloat(0.9f, 1.5f));
            e.innerColor = Color.White;
            e.outerColor = Color.SkyBlue;
            e.fadeToColor = Color.DarkBlue;
        }
    }
}