using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Content.Dialogue;
using Stellamod.Core.Camera;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;

public class VerliaPrison : ModNPC,
    INPCSpawnCondition
{
    private ref float Timer => ref NPC.ai[0];
    private ref float WiggleTimer => ref NPC.ai[1];
    private ref float IsDying => ref NPC.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        NPCSets.Heavy[Type] = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 999999;
        NPC.lifeMax = 5;
        NPC.HitSound = SoundID.DD2_CrystalCartImpact with { PitchVariance = 0.5f };
        NPC.DeathSound = SoundID.DD2_WitherBeastCrystalImpact;
        NPC.knockBackResist = 0f;
        NPC.dontCountMe = true;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Vector2 velocity = new Vector2();
        velocity.Y = MathF.Sin(Timer * 0.05f) * 0.2f;
        NPC.velocity = velocity;
        if (DownedBossTracker.IsDowned(DownedBossFlag.VerliaPrison))
            NPC.active = false;

        if (Main.rand.NextBool(16))
        {
            var sp = SparkleParticle.Spawn(
                NPC.Center + Main.rand.NextVector2Circular(64, 64),
                -Vector2.UnitY);
            sp.Scale *= 0.4f;
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.outerColor = Color.Blue;
        }

        if (WiggleTimer > 0)
            WiggleTimer--;

        float radians = MathHelper.Lerp(0f, 0.28f, WiggleTimer / 15f);
        NPC.rotation = MathHelper.Lerp(-radians, radians,
            MathF.Sin(WiggleTimer * 0.5f) * 0.5f + 0.5f);

        if(IsDying > 0)
        {
            AI_Dying();
        }
    }

    private void AI_Dying()
    {
        ShakeScreenPosition.Shake = 3;
        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(120);
        if (IsDying % 8 == 0)
        {
            Vector2 pos = NPC.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 250);
            Vector2 vel = (NPC.Center - pos) * 0.03f;
            SparkleParticle sp = SparkleParticle.Spawn(pos, vel, Color.Red, 0.3f);
            sp.outerColor = Color.Blue;
            sp.innerColor = Color.White;
            sp.fast = true;
            sp.noTileCollide = true;
            sp.gravity = 0;

            pos = NPC.Center + Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(100, 250);
            vel = (NPC.Center - pos) * 0.03f;
            var gp = FXUtil.GlowStretch(pos, vel);
            gp.OuterGlowColor = Color.Blue;
        }
        IsDying++;
        if (IsDying == 2)
        {
            SoundStyle summonSound = new SoundStyle("Stellamod/Assets/Sounds/RisingSummon");
            SoundEngine.PlaySound(summonSound, NPC.position);
        }
        if(IsDying % 30 == 0)
        {
            PixelPrimitiveCircleFactory.CreateVerliaMoonBoom2(NPC.Center);
        }

        if (IsDying >= 165)
        {
            ShakeScreenPosition.Shake = 16;
            FXUtil.ShakeCamera(NPC.Center, 2048, 32);
            if (Main.netMode != NetmodeID.Server)
            {
                ShockwavePlayer shockwavePlayer = Main.LocalPlayer.GetModPlayer<ShockwavePlayer>();
                shockwavePlayer.Bee = 180;
                shockwavePlayer.shockwavePosition = NPC.Center;
                shockwavePlayer.rippleSize = 5;
                DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
                DelayHelper.Invoke(120, () => dialogueSystem.StartDialogueSequence(ModContent.GetInstance<VerliaFreeingDialogue>()));

                int headGore = Mod.Find<ModGore>($"{Name}_Gore_0").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore_1").Type;
                int legGore2 = Mod.Find<ModGore>($"{Name}_Gore_2").Type;

                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore2);
            }


            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 35, baseSize: 0.24f);
            for (float f = 0; f < 12f; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(18, 18);
                var dp = DustParticle.Spawn(NPC.Center, velocity);
                dp.Scale *= 0.75f;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.outerColor = Color.Blue;
            }
            for (float f = 0; f < 4f; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(18, 18);
                var dp = FXUtil.GlowStretch(NPC.Center, velocity);
                dp.VectorScale *= 0.5f;
            }

            var part = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.2f);
            part.Scale *= 6;

            var part3 = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightBlue, Color.Purple, baseSize: 0.15f);
            part3.Scale *= 4;

            var part2 = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightBlue, Color.Purple);
            part2.Scale *= 3;
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), NPC.position);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/STARGROP"), NPC.position);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb"), NPC.position);

            for (float f = 0; f < 42; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(128, 128);
                FXUtil.GlowStretch(NPC.Center, velocity);
            }
            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightCyan,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
                particle.Scale *= 4;
            }

            var b = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 45, baseSize: 0.2f);
            b.Scale *= 2;


            float numDust = 48;
            for (float f = 0; f < numDust; f++)
            {
                Vector2 vel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(8f, 12f);
                SparkleParticle sp = SparkleParticle.Spawn(NPC.Center, vel, Color.Blue, Main.rand.NextFloat(0.6f, 1f));
                sp.outerColor = Color.Blue;
                sp.innerColor = Color.White;
                sp.fast = true;
                sp.dampening = 0.05f;
                sp.noTileCollide = true;
                sp.gravity = 0;
            }
            for (float f = 0; f < numDust; f++)
            {
                Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemSapphire, Main.rand.NextVector2Circular(24, 24), Scale: Main.rand.NextFloat(0.6f, 2f));
                d.noGravity = true;
            }
            NPC.Kill();
        }

    }
    private void RenderGlowingBall(SpriteBatch sb, Vector2 sp)
    {
        Asset<Texture2D> glowBallAsset = AssetManager.GlowMask.SimpleGlowCircle;
        float ratio = IsDying / 165;
        float ease = EasingFunction.InOutSine(ratio);
        Color color = Color.Lerp(Color.Blue * 0.5f, Color.Blue, ease);
        Vector2 scale = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
        SpritebatchDrawer dw = SpritebatchDrawer.FromTextureAsset(glowBallAsset, NPC.Center);
        dw.color = color;
        dw.color.A = 0;
        dw.scale = scale;
        sb.Draw(dw);
        dw.color = Color.White;
        dw.color.A = 0;
        dw.scale *= 0.5f;
        sb.Draw(dw);
    }
    public bool CanSpawn()
    {
        return
            !NPC.AnyNPCs(ModContent.NPCType<VerliaPrison>()) &&
            !DownedBossTracker.IsDowned(DownedBossFlag.VerliaPrison);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        WiggleTimer = 15;
        if (NPC.life <= 0)
        {
            if(IsDying < 1)
                IsDying = 1;
            NPC.life = 1;
        }
        else
        {
            for (float f = 0; f < 6f; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(13, 13);
                var dp = DustParticle.Spawn(NPC.Center, velocity);
                dp.Scale *= 0.5f;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.outerColor = Color.Blue;
            }
        }
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if(IsDying > 1)
        {
            PixelationManager.QueueSpritebatchDrawAction(RenderGlowingBall);
        }
        NPC.spriteDirection = 1;

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.5f, 1f, speed: 3);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.65f;
        spriteBatch.Draw(glowDrawer);
        SpritebatchDrawer crystalDrawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(crystalDrawer);

        crystalDrawer.VerticalFrame(2, 3);
        crystalDrawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 1f, speed: 3));
        crystalDrawer.color.A = 0;
        spriteBatch.Draw(crystalDrawer);
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.VerliaPrison);
    }
}
