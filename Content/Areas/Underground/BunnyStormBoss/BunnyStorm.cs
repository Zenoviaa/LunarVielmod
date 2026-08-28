using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Effects.GothinFlames;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.BunnyStormBoss;




public class BunnyStormBunny : ModProjectile
{
    private Vector2 _scale;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 6;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    public override bool CanHitPlayer(Player target)
    {
        if (Style == 2)
            return false;
        return base.CanHitPlayer(target);
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        switch (Style)
        {
            case 0:
                _scale = Vector2.One;
                AI_Falling();
                break;
            case 1:
                _scale = Vector2.One;
                AI_ShootOut();
                break;
            case 2:
                AI_Suck();
                break;
        }
        //  TileCollideWhenBelowPlayer();
    }

    private void TileCollideWhenBelowPlayer()
    {
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 2048);
        if (player != null && Projectile.Bottom.Y > player.Top.Y && Projectile.velocity.Y > 5)
            Projectile.tileCollide = true;
    }

    private void AI_Suck()
    {
        _scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InOutSine(Timer / 30f));
        Vector2 velocityToParent = (Parent.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
        velocityToParent = velocityToParent.RotatedBy(0.5f);
        Projectile.velocity = velocityToParent * 12;

        if (Vector2.Distance(Projectile.Center, Parent.Center) < 16)
        {
            Projectile.Kill();
        }


        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 2)
                Projectile.frame = 0;
        }
        Projectile.rotation += 0.05f;
    }

    private void AI_ShootOut()
    {
        if (Timer == 1)
        {
            var p = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.OrangeRed, Color.Black);
            p.Scale *= Main.rand.NextFloat(0.4f, 0.65f);

            var sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center, Projectile.velocity * 0.2f, Color.DarkGray);
            sp.initialColor = Color.Lerp(Color.Red, Color.Black, 0.6f);
            sp.fast = true;

            MuzzleFlashParticle flashParticle = MuzzleFlashParticle.Spawn(Projectile.Center, Projectile.velocity, Color.Yellow);
            flashParticle.innerColor = Color.Yellow;
            flashParticle.bloomColor = Color.OrangeRed;
            flashParticle.Scale *= Main.rand.NextFloat(0.15f, 0.3f);


            FaintSmokeParticle faintSmoke = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center, Projectile.velocity, Scale: Main.rand.NextFloat(0.2f, 0.4f));
            faintSmoke.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
            faintSmoke.fadeToColor = Color.DarkGray;
            faintSmoke.Scale = Main.rand.NextFloat(0.15f, 0.3f);
            for (float f = 0; f < 4; f++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    gravity = 0f,
                    innerColor = Color.Yellow,
                    outerColor = Color.OrangeRed,
                    scaleRange = new Vector2(0.3f, 1f)
                };
                var dp = DustParticle.Spawn(Projectile.Center, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
                dp.dampening = 0.1f;
            }

            SoundStyle gunShotSound = new SoundStyle("Stellamod/Assets/Sounds/GunShot2") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(gunShotSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 4;
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.3f);
            donut.Scale *= 0.5f;
            float numDust = 8;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = Projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(35));
                var dp = DustParticle.Spawn(Projectile.Center, vel);
                dp.outerColor = Color.DarkGray;
                dp.Scale *= 0.6f;
            }
        }

        Projectile.rotation += 0.015f;
        if (Timer % 8 == 0)
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Projectile.width);
            pos.Y = Main.rand.Next(0, Projectile.height);
            pos += Projectile.position;
            var sp = SparkleParticle.Spawn(pos, -Vector2.UnitY * 0.3f, Scale: 0.3f);
            sp.outerColor = Color.SkyBlue;
            sp.noTileCollide = true;
            sp.gravity = 0;
        }

        if (Timer % 24 == 0)
        {
            var fx = FXUtil.GlowStretch(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity * 0.05f);
            fx.VectorScale *= 0.5f;
        }


        Projectile.rotation += Projectile.velocity.Length() * 0.025f;
        Projectile.rotation += 0.015f;
    }

    private void AI_Falling()
    {
        if (Timer % 8 == 0)
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Projectile.width);
            pos.Y = Main.rand.Next(0, Projectile.height);
            pos += Projectile.position;
            var sp = SparkleParticle.Spawn(pos, -Vector2.UnitY * 0.3f, Scale: 0.3f);
            sp.outerColor = Color.SkyBlue;
            sp.noTileCollide = true;
            sp.gravity = 0;
        }

        if (Timer % 24 == 0)
        {
            var fx = FXUtil.GlowStretch(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity * 0.05f);
            fx.VectorScale *= 0.5f;
        }
        if (Projectile.velocity.Y < 15)
            Projectile.velocity.Y += 0.15f;
        Projectile.rotation += Projectile.velocity.Length() * 0.025f;
        Projectile.rotation += 0.015f;

        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 2)
                Projectile.frame = 0;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = pos;
            afDrawer.rotation = Projectile.oldRot[i];
            afDrawer.color = Color.Lerp(Color.White, Color.Black, i / (float)Projectile.oldPos.Length) * 0.05f;
            afDrawer.color.A = 0;
            afDrawer.scale = _scale;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale = _scale;

        Main.spriteBatch.Draw(sbDrawer);

        if (Style == 2)
            return false;
        sbDrawer.VerticalFrame(Projectile.frame + 4, 6);
        sbDrawer.color = Color.Red;
        Main.spriteBatch.Draw(sbDrawer);

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (Main.netMode == NetmodeID.Server)
            return;

        int headGore = Mod.Find<ModGore>($"BunnyStorm_Gore").Type;
        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(8, 8) + new Vector2(0, -8), headGore, 1f);
    }
}
public class BunnyStormCrystal : ModNPC,
    INPCSpawnCondition
{
    private ref float Timer => ref NPC.ai[0];
    private ref float WiggleTimer => ref NPC.ai[1];
    private ref float IsDying => ref NPC.ai[2];
    public override string Texture => ModContent.GetInstance<BunnyStorm>().Texture;
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
        if (DownedBossTracker.IsDowned(DownedBossFlag.BunnyStorm))
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

        if (IsDying > 0)
        {
            AI_Dying();
        }
    }

    private void AI_Dying()
    {
        ShakeScreenPosition.Shake = 2;
        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(30);
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
        if (IsDying % 30 == 0)
        {

        }

        if (IsDying >= 30)
        {
            ShakeScreenPosition.Shake = 16;
            FXUtil.ShakeCamera(NPC.Center, 2048, 32);
            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BunnyStorm>());
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
            !NPC.AnyNPCs(ModContent.NPCType<BunnyStorm>()) &&
            !NPC.AnyNPCs(ModContent.NPCType<BunnyStormCrystal>()) &&
            !DownedBossTracker.IsDowned(DownedBossFlag.BunnyStorm);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        WiggleTimer = 15;
        if (NPC.life <= 0)
        {
            if (IsDying < 1)
            {
                IsDying = 1;
                FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 35, baseSize: 0.24f);
                PixelPrimitiveCircleFactory.CreateVerliaMoonBoom2(NPC.Center);
                NPC.netUpdate = true;
            }

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
        if (IsDying > 1)
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
    }
}


public class BunnyStorm : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Hand_Wiggle_Bunny_Drop,
        Bunny_Fist,
        Bunny_Gun,
        Bunny_Remerge,
    }


    private int _stormFrame;
    private float _stormRotation;
    private float _startRotation;
    private bool _contactDamage;
    private bool _showTrail;
    private float _trailAlpha;
    private Vector2 _initialVelocity;
    private Vector2 _boundPoint;
    private Vector2 _stormScale;
    private Outliner _outliner;
    private Asset<Texture2D> _bunnyNoiseTextureAsset;
    private Asset<Texture2D> _bunnyMaskTextureAsset;
    private Asset<Texture2D> _earTextureAsset;
    private Asset<Texture2D> _whiskersTextureAsset;
    private VortexParticleSystem _vortexPS;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private PatternManager<AIState> _patternManagerBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new();
                _patternManagerBackingField.AddPattern(AIState.Hand_Wiggle_Bunny_Drop, 1f);
                _patternManagerBackingField.AddPattern(AIState.Bunny_Fist, 1f);
                _patternManagerBackingField.AddPattern(AIState.Bunny_Gun, 1f);
            }

            return _patternManagerBackingField;
        }
    }

    private float IdleTime => 400;
    private float SpawnTime => 200;
    private float DespawnTime => 100f;

    private float MaxIdleHoverDistance => 180;

    private int ShockwaveDamage => 24;
    private int GunDamage => 36;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_boundPoint);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _boundPoint = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 3;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
      
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 100;
        NPC.height = 100;
        NPC.damage = 50;
        NPC.defense = 10;
        NPC.lifeMax = 1700;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DeadlyFoe");
        }
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    private void SimulateBunnyParticles()
    {
        _vortexPS ??= new VortexParticleSystem(64);
        _vortexPS.centerPoint = -NPC.velocity * 16;
        for (int i = 0; i < 2; i++)
        {

            _vortexPS.SpawnParticle(Vector2.Zero + Main.rand.NextVector2CircularEdge(64, 64), Main.rand.NextVector2Circular(2, 2));
        }

        _vortexPS.Update();
    }

    public override void AI()
    {
        base.AI();
        SimulateBunnyParticles();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
                SwitchState(AIState.Despawn);
        }

        Lighting.AddLight(NPC.Center, TorchID.Ice);

        _showTrail = false;
        _contactDamage = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Death:
                AI_Death();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Hand_Wiggle_Bunny_Drop:
                AI_HandWiggleBunnyDrop();
                break;
            case AIState.Bunny_Fist:
                AI_BunnyFist();
                break;
            case AIState.Bunny_Gun:
                AI_BunnyGun();
                break;
            case AIState.Bunny_Remerge:
                AI_BunnyRemerge();
                break;
        }
        _outliner.Update();
        _trailAlpha = MathHelper.Lerp(_trailAlpha, _showTrail ? 1f : 0f, 0.1f);
    }


    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
        }
    }

    private void PlayReadySound()
    {
        SoundStyle readySound = new SoundStyle($"Stellamod/Assets/Sounds/OverGrowth_TP{Main.rand.Next(1, 3)}");
        readySound = readySound with { PitchVariance = 0.5f };
        readySound.Volume = 0.6f;
        SoundEngine.PlaySound(readySound, NPC.position);
    }

    private void AI_BunnyRemerge()
    {

    }

    private void AI_BunnyGun()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        PlayReadySound();
                        _initialVelocity = NPC.velocity;
                        NPC.TargetClosest();
                    }
                    _outliner.warning = true;

                    float speedUp = MathHelper.Lerp(1f, 0.75f, AttackCounter / 5f);
                    float fistingTime = 120 * speedUp;
                    float ratio = Timer / fistingTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 s1 = Vector2.Lerp(Vector2.One, Vector2.Zero, ease);
                    Vector2 s2 = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
                    _stormScale = Vector2.Lerp(s1, s2, ease);


                    if (Timer >= fistingTime / 2f)
                    {
                        _stormFrame = 1;
                    }

                    float inTime = 90 * speedUp;
                    float ratio2 = Timer / inTime;
                    Vector2 upOffset = Vector2.UnitY * MathHelper.Lerp(352, 232, EasingFunction.InOutSine(ratio2));
                    upOffset = upOffset.RotatedBy((AttackCounter / 6f) * MathHelper.TwoPi);
                    Vector2 startupPoint = MyTarget.Center - upOffset;

                    //    startupPoint -= Vector2.UnitX * MathHelper.Lerp(0, 300, EasingFunction.InOutSine(ratio2));
                    startupPoint.Y -= MathHelper.Lerp(0f, 100, EasingFunction.InExpo(ratio));
                    Vector2 velocityToPoint = (startupPoint - NPC.Center);
                    NPC.velocity = Vector2.Lerp(_initialVelocity, velocityToPoint, EasingFunction.InOutExpo(ratio2));


                    _stormRotation = MathHelper.Lerp(0, MathHelper.TwoPi * 2, EasingFunction.InOutExpo(ratio)) + MathHelper.PiOver2;
                    if (Timer >= fistingTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage1");
                        boom2.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom2, MyTarget.position);
                        _stormFrame = 1;
                        _startRotation = _stormRotation;
                    }

                    Vector2 slowMoveVelocity = (MyTarget.Center - NPC.Center);

                    float speed = MathHelper.Lerp(0.5f, 9f, Vector2.Distance(NPC.Center, MyTarget.Center) / 384f);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, slowMoveVelocity.SafeNormalize(Vector2.Zero) * speed, EasingFunction.InOutSine(Timer / 24f));
                    _outliner.warning = true;
                    float targetRotation = (MyTarget.Center - NPC.Center).ToRotation();


                    _stormRotation = Utils.AngleLerp(_startRotation, targetRotation, EasingFunction.InOutSine(Timer / 24f));
                    if (Timer >= 24f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    Vector2 fireVelocity = _stormRotation.ToRotationVector2() * 12;
                    Vector2 offset = new Vector2(32, -57);
                    Vector2 firePoint = NPC.Center + offset.RotatedBy(_stormRotation);
                    if (Timer == 1)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            CreateBunnyGore(firePoint, fireVelocity.RotatedByRandom(0.8f) * 0.5f);
                        }

                        _startRotation = _stormRotation;
                    }

                    _stormRotation = Utils.AngleLerp(_startRotation, _startRotation - MathHelper.ToRadians(60), EasingFunction.QuadraticBump(Timer / 25f));
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), firePoint, (MyTarget.Center - firePoint).SafeNormalize(Vector2.Zero) * 12,
                            ModContent.ProjectileType<BunnyStormBunny>(), GunDamage, 1, Main.myPlayer, ai1: 1);
                    }
                    Vector2 s1 = Vector2.Lerp(Vector2.One, new Vector2(0.5f), EasingFunction.OutExpo(Timer / 50f));
                    Vector2 s2 = Vector2.Lerp(new Vector2(0.5f), Vector2.One, EasingFunction.InOutSine(Timer / 50f));
                    Vector2 s = Vector2.Lerp(s1, s2, Timer / 50f);
                    _stormScale = s;
                    float dir = AttackCounter % 2 == 0 ? 1 : -1;
                    _startRotation += MathHelper.TwoPi / 50f * dir;
                    NPC.velocity += ((NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(4f, 0f, EasingFunction.OutExpo(Timer / 50f)));
                    NPC.velocity *= 0.8f;
                    if (Timer >= 25)
                    {
                        _stormFrame = 0;
                    }

                    if (Timer >= 50)
                    {

                        AttackCounter++;
                        if (AttackCounter >= 7)
                        {

                            SwitchState(AIState.Idle);
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle--;
                        }
                    }
                }
                break;
        }
    }
    private bool IsGrounded()
    {
        Point solidTileBelow = NPC.Bottom.ToTileCoordinates();
        solidTileBelow.Y += 2;
        bool tileSolid = Main.tileSolid[Main.tile[solidTileBelow].TileType] || Main.tileSolidTop[Main.tile[solidTileBelow].TileType];
        bool isGrounded = Main.tile[solidTileBelow].HasTile && tileSolid;
        return isGrounded;

    }
    private void AI_BunnyFist()
    {
        _showTrail = true;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {

                        PlayReadySound();
                        _initialVelocity = NPC.velocity;
                        NPC.TargetClosest();
                    }

                    float speedUp = MathHelper.Lerp(1f, 0.75f, AttackCounter / 3f);
                    float fistingTime = 150 * speedUp;
                    if (AttackCounter >= 3)
                    {
                        fistingTime *= 2f;
                    }
                    float ratio = Timer / fistingTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 s1 = Vector2.Lerp(Vector2.One, Vector2.Zero, ease);
                    Vector2 s2 = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
                    _stormScale = Vector2.Lerp(s1, s2, ease);


                    float inTime = 120f * speedUp;
                    if (AttackCounter >= 3)
                    {
                        inTime *= 2f;
                    }
                    float ratio2 = Timer / inTime;

                    float mult = AttackCounter >= 3 ? 4 : 1;

                    Vector2 startupPoint = _boundPoint - Vector2.UnitY * MathHelper.Lerp(100, 50, EasingFunction.InOutSine(ratio2)) * mult;
                    float dir = AttackCounter % 2 == 0 ? 1 : -1;
                    if (AttackCounter >= 3)
                        dir = 0;
                    startupPoint -= Vector2.UnitX * MathHelper.Lerp(0, 300, EasingFunction.InOutSine(ratio2)) * dir;
                    startupPoint.Y -= MathHelper.Lerp(0f, 100, EasingFunction.InExpo(ratio));
                    Vector2 velocityToPoint = (startupPoint - NPC.Center);

                    NPC.velocity = Vector2.Lerp(_initialVelocity, velocityToPoint, EasingFunction.InOutQuad(ratio2));

                    if (Timer >= fistingTime / 2f)
                    {
                        _stormFrame = 3;
                    }
                    _outliner.warning = true;


                    _stormRotation = MathHelper.Lerp(0, MathHelper.TwoPi * 2, EasingFunction.InOutQuad(ratio)) + MathHelper.PiOver2;
                    if (Timer >= fistingTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }

                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage2");
                        boom2.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom2, MyTarget.position);
                    }
                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (NPC.velocity.Y < 25)
                        NPC.velocity.Y += MathHelper.Lerp(0.05f, 4f, EasingFunction.InExpo(Timer / 60f));
                    else
                    {
                        if (Timer % 4 == 0)
                        {
                            LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -Vector2.UnitY);
                        }

                        NPC.velocity.Y *= 1.05f;
                    }
                    if (NPC.Bottom.Y < MyTarget.Top.Y)
                        NPC.noTileCollide = true;
                    else
                        NPC.noTileCollide = false;
                    if ((IsGrounded() && Timer > 15) || Timer >= 80)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        FXUtil.ShakeCamera(NPC.Center, 1024, 8);

                        SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
                        boom.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom, NPC.position);
                        for (int i = 0; i < 16; i++)
                        {
                            float radius = 150;
                            Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                            offset *= Main.rand.NextFloat(1f, radius);
                            offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                            velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                                Main.rand.NextFloat(0.3f, 0.7f));
                        }

                        FXUtil.GlowCircleBoom(NPC.Bottom,
                           innerColor: Color.White,
                           glowColor: Color.Black,
                           outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
                        for (float i = 0; i < 4; i++)
                        {
                            float progress = i / 4f;
                            float rot = progress * MathHelper.ToRadians(240);
                            Vector2 offset = rot.ToRotationVector2() * 24;
                            var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                                innerColor: Color.White,
                                glowColor: Color.Black,
                                outerGlowColor: Color.Black,
                                baseSize: 0.24f);
                            particle.Rotation = rot + MathHelper.ToRadians(45);
                        }

                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                            var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                            particle.InnerColor = Color.White;
                            particle.GlowColor = Color.LightCyan;
                            particle.OuterGlowColor = Color.Black;
                            particle.Duration = Main.rand.NextFloat(25, 50);
                            particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                            particle.VectorScale *= 0.5f;
                        }


                        if (AttackCounter == 3)
                        {
                            ShakeScreenPosition.Shake = 16;
                            FXUtil.ShakeCamera(NPC.position, 1024, 129);
                            SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/RocketExplosion");
                            boom2.PitchVariance = 0.3f;
                            SoundEngine.PlaySound(boom2, MyTarget.position);
                        }
                        if (MultiplayerHelper.IsHost)
                        {
                            int shockwaveDamage = ShockwaveDamage;
                            int knockback = 1;
                            if (AttackCounter == 3)
                            {
                                //This is the part where you spawn the cool ahh shockwaves
                                //But we have to make cool ahh shockwaves :(

                                Vector2 velocity = Vector2.UnitX;
                                velocity *= 4;

                                Point tile = TileUtilities.FallToSolidTile(NPC.Top.ToTileCoordinates());
                                tile.Y -= 4;
                                Vector2 pos = tile.ToWorldCoordinates();
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, velocity,
                                    ModContent.ProjectileType<SuperWindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                                velocity = -velocity;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, velocity,
                               ModContent.ProjectileType<SuperWindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                            }
                            else
                            {
                                //This is the part where you spawn the cool ahh shockwaves
                                //But we have to make cool ahh shockwaves :(

                                Vector2 velocity = Vector2.UnitX;
                                velocity *= 4;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                                    ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                                velocity = -velocity;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                               ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                            }

                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 fireVelocity = Vector2.Lerp(-Vector2.UnitX, Vector2.UnitX, i / 8f) * 8;
                                fireVelocity.Y -= 12;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, fireVelocity,
                                    ModContent.ProjectileType<BunnyStormBunny>(), shockwaveDamage, knockback, Main.myPlayer, ai1: 0);
                            }
                        }
                    }

                    ShakeScreenPosition.Shake = MathHelper.Lerp(8, 1, EasingFunction.InOutSine(Timer / 100f));


                    NPC.velocity.Y = 0;
                    NPC.noTileCollide = true;

                    if (AttackCounter < 3)
                    {
                        Vector2 s1 = Vector2.Lerp(Vector2.One, new Vector2(0.5f), EasingFunction.OutExpo(Timer / 50f));
                        Vector2 s2 = Vector2.Lerp(new Vector2(0.5f), Vector2.One, EasingFunction.InOutSine(Timer / 50f));
                        Vector2 s = Vector2.Lerp(s1, s2, Timer / 50f);
                        _stormScale = s;
                        if (Timer >= 25)
                        {
                            _stormFrame = 0;
                        }
                        if (Timer >= 50)
                        {

                            Timer = 0;
                            AttackCycle = 0;
                            AttackCounter++;
                        }

                    }
                    else
                    {
                        if (Timer >= 100)
                        {
                            SwitchState(AIState.Idle);
                        }
                    }

                }
                break;
        }
    }


    private void AI_HandWiggleBunnyDrop()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        PlayReadySound();
                        _initialVelocity = NPC.velocity;
                        NPC.TargetClosest();
                    }

                    float fistingTime = 160;
                    float ratio = Timer / fistingTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 s1 = Vector2.Lerp(Vector2.One, Vector2.Zero, ease);
                    Vector2 s2 = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
                    _stormScale = Vector2.Lerp(s1, s2, ease);

                    if (Timer >= fistingTime / 2f)
                    {
                        _stormFrame = 2;
                    }

                    float inTime = 120;
                    float ratio2 = Timer / inTime;
                    Vector2 upOffset = Vector2.UnitY * MathHelper.Lerp(352, 232, EasingFunction.InOutSine(ratio2));
                    Vector2 startupPoint = MyTarget.Center - upOffset;
                    _stormRotation *= 0.98f;
                    //    startupPoint -= Vector2.UnitX * MathHelper.Lerp(0, 300, EasingFunction.InOutSine(ratio2));
                    startupPoint.Y -= MathHelper.Lerp(0f, 100, EasingFunction.InOutQuad(ratio));
                    Vector2 velocityToPoint = (startupPoint - NPC.Center);
                    NPC.velocity = Vector2.Lerp(_initialVelocity, velocityToPoint, EasingFunction.InOutQuad(ratio2));
                    if (Timer >= fistingTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    Vector2 startupPoint = MyTarget.Center - Vector2.UnitY * 352;
                    Vector2 movement = (startupPoint - NPC.Center);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, movement, 0.2f);
                    _stormRotation = Utils.AngleLerp(_stormRotation, NPC.velocity.X * 0.02f, 0.05f);

                    if (Timer % 15 == 0)
                    {
                        SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/BasicMagicHit2");
                        boom2.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom2, MyTarget.position);
                        _stormScale *= 1.1f;
                        if (MultiplayerHelper.IsHost)
                        {
                            ProjFirer firer = ProjFirer.From<BunnyStormBunny>(NPC);
                            Vector2 offset = new Vector2();
                            offset.X = Main.rand.NextFloat(-64, 64);
                            firer.position += offset;
                            firer.damage = ShockwaveDamage;
                            firer.New();
                        }
                    }

                    if (Timer >= 400)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                    Vector2 targetScale = Vector2.Lerp(Vector2.One, Vector2.Zero, Timer / 400f);
                    _stormScale = Vector2.Lerp(_stormScale, targetScale, 0.2f);
                    _outliner.warning = true;
                    NPC.velocity *= 0.9f;
                }
                break;
            case 2:
                {
                    _stormFrame = 0;
                    if (Timer == 1)
                    {
                        SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirageRed");
                        boom2.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom2, MyTarget.position);
                    }
                    if (Timer % 10 == 0 && Timer < 60)
                    {
                        PixelPrimitiveCircleFactory.CreateInWhiteSuck(NPC.Center);
                    }
                    ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 3f, EasingFunction.QuadraticBump(Timer / 80f));
                    NPC.velocity *= 0.9f;
                    if (Timer == 59)
                    {
                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.LightGray, Color.DarkGray, duration: 25, baseSize: 0.24f);
                        fx.Scale *= 1.2f;
                        float numDust = 12;
                        for (float n = 0; n < numDust; n++)
                        {
                            var d = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(16, 16));
                            d.noTileCollide = true;
                            d.gravity = 0;
                            d.dampening = 0.05f;
                        }


                    }
                    _stormScale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InExpo(Timer / 80f));
                    if (Timer >= 80f)
                    {
                        SoundStyle boom2 = new SoundStyle("Stellamod/Assets/Sounds/GSummon");
                        boom2.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom2, MyTarget.position);
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    public override bool AllowNameplateToBeShown()
    {
        return State != AIState.Spawn;
    }

    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            _initialVelocity = NPC.velocity;
            NPC.TargetClosest();
        }

        //For the idle state, the storm moves around its binding point semi randomly, but it follows a nice path, probably just have it circle around it
        Vector2 vecFromBind = (NPC.Center - _boundPoint);
        vecFromBind = vecFromBind.RotatedBy(0.015);
        if (vecFromBind.Length() > MaxIdleHoverDistance)
        {
            vecFromBind = vecFromBind.Resize(MaxIdleHoverDistance);
        }
        else if (vecFromBind.Length() < 1)
        {
            vecFromBind += Vector2.UnitY * MaxIdleHoverDistance * 0.5f;
        }


        //  vecFromBind *= MathHelper.Lerp(0.5f, 1f, MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f);
        Vector2 newPoint = _boundPoint + vecFromBind;
        Vector2 velocityToNewPoint = (newPoint - NPC.Center);
        NPC.velocity = Vector2.Lerp(_initialVelocity, velocityToNewPoint, EasingFunction.InOutQuad(Timer / 240f));
        NPC.rotation = NPC.velocity.X * 0.05f;
        _stormFrame = 0;
        _stormScale = Vector2.Lerp(_stormScale, Vector2.One, 0.1f);
        _stormRotation = Utils.AngleLerp(_stormRotation, 0, 0.02f);
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
    private void AI_Spawn()
    {
        Timer++;
        if (Timer == 1)
        {
            _boundPoint = NPC.Center;
            NPC.netUpdate = true;
        }

        if (Timer % 10 == 0 && MultiplayerHelper.IsHost)
        {
            ProjFirer firer = ProjFirer.From<BunnyStormBunny>(NPC);
            firer.ai1 = 2;
            firer.ai2 = NPC.whoAmI;
            firer.position += Main.rand.NextVector2CircularEdge(600, 600);
            firer.New();
        }

        CreateSuckLines();
        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(120);
        NPC.velocity.Y = MathHelper.Lerp(-1f, 0f, EasingFunction.InOutSine(Timer / SpawnTime));
        _stormScale = Vector2.Lerp(Vector2.Zero, Vector2.One, Timer / SpawnTime);
        if (Timer >= SpawnTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void CreateSuckLines()
    {

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(252, 512);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(384, 666);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);


            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.25f;
        }

    }
    public void CreateBunnyGore(Vector2 position, Vector2 velocity)
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        int headGore = Mod.Find<ModGore>($"{Name}_Gore").Type;
        Gore.NewGore(NPC.GetSource_Death(), position, velocity, headGore, 1f);
    }

    private void AI_Death()
    {
        Timer++;
        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(252, 512);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (pos - NPC.Center);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(384, 666);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (pos - NPC.Center);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.25f;
        }
        if (Timer % 7 == 0)
        {
            CreateBunnyGore(NPC.Center, Main.rand.NextVector2Circular(16, 16));
            _stormScale *= 1.25f;
        }

        NPC.velocity *= 0.8f;

        Vector2 targetScale = Vector2.Lerp(Vector2.One, Vector2.Zero, EasingFunction.InOutSine(Timer / 180f));
        _stormScale = Vector2.Lerp(_stormScale, targetScale, 0.1f);
        _stormFrame = 0;
        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(120);
        if (Timer >= 180)
        {
            for (int i = 0; i < 64; i++)
            {
                CreateBunnyGore(NPC.Center, Main.rand.NextVector2Circular(16, 16));
                _stormScale *= 1.25f;
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), NPC.position);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb"), NPC.position);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 35, baseSize: 0.24f);
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 60, 256);
            FXUtil.GlowCircleBoom(NPC.Center,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(NPC.Center, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }
            NPC.Kill();
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y -= 0.3f;
        if (Timer >= DespawnTime)
        {
            NPC.active = false;
        }
    }


    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }
    public override BossLevel GetBossLevel()
    {
        return BossLevel.Miniboss;
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            NPC.life = 1;
            if (State != AIState.Death)
            {
                SwitchState(AIState.Death);
            }
        }
    }
    private void DrawMask(SpriteBatch sb, int frameOffset)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, NPC.Center);
        maskDrawer.VerticalFrame(_stormFrame + frameOffset, 12);
        maskDrawer.CenterOrigin();
        maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3);
        maskDrawer.rotation = _stormRotation;
        sb.Draw(maskDrawer);

    }
    private void DrawMask(SpriteBatch sb, int frameOffset, Color color)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");

        FlameBowShader flamebowShader = ShaderContent.GetInstance<FlameBowShader>();
        flamebowShader.Time = Main.GlobalTimeWrappedHourly * -24;
        flamebowShader.FlameNoiseTexture = AssetManager.Noise.InvertedVoronoi;
        flamebowShader.InsideColor = Color.White;
        flamebowShader.BloomColor = Color.Cyan;
        flamebowShader.DissipateThreshold = MathHelper.Lerp(1f, 0f, 0.75f);
        flamebowShader.DistortionStrength = 0.1f;

        sb.Restart(effect: flamebowShader.Effect, samplerState: SamplerState.AnisotropicClamp);
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, NPC.Center);
        maskDrawer.VerticalFrame(_stormFrame + frameOffset, 4);
        maskDrawer.CenterOrigin();
        maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3) * 1.1f;
        maskDrawer.rotation = _stormRotation;
        maskDrawer.color = color * 0.3f;
        maskDrawer.color.A = 0;
        sb.Draw(maskDrawer);

        maskDrawer.color = Color.Violet * 0.3f;
        maskDrawer.color.A = 0;
        sb.Draw(maskDrawer);


        maskDrawer.color = Color.LightBlue * ExtraMath.Osc(0.8f, 1.2f, speed: 4);
        maskDrawer.color.A = 0;
        sb.Draw(maskDrawer);
        sb.RestartDefaults();

    }

    private void DrawGlow(SpriteBatch spriteBatch, Vector2 screenPos)
    {


        DrawMask(spriteBatch, 0, Color.White);
    }
    private void DrawCrystal(SpriteBatch spriteBatch)
    {

        NPC.spriteDirection = 1;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = NPC.Center;
        drawer.worldPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 3);
        drawer.rotation += NPC.velocity.X * 0.05f;
        spriteBatch.Draw(drawer);

        drawer.color = Color.Green * ExtraMath.Osc(0.1f, 0.25f, speed: 2);
        drawer.color.A = 0;
        drawer.VerticalFrame(1, 3);
        spriteBatch.Draw(drawer);



        drawer.color = Color.LightBlue * ExtraMath.Osc(0.6f, 1f, speed: 2) * 0.4f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

    }
    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _trailAlpha;
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 64, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.DarkGray;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
    private void DrawBunnyParticles(SpriteBatch spriteBatch)
    {
        if (_vortexPS == null)
            return;

        SpritebatchDrawer bunnyDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BunnyStormBunny>()], NPC.Center);
        bunnyDrawer.VerticalFrame(0, 6);
        bunnyDrawer.CenterOrigin();
        bunnyDrawer.color = _outliner.outlineColor;
        for (int i = 0; i < _vortexPS.particles.Length; i++)
        {
            ref Vector2 pos = ref _vortexPS.particles.positions[i];
            bunnyDrawer.worldPosition = NPC.Center + pos;
            bunnyDrawer.rotation = Main.GlobalTimeWrappedHourly * 4 + i * 8;
            spriteBatch.Draw(bunnyDrawer);
        }
    }
    private void RenderBunnyParticles(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_vortexPS == null)
            return;
        SpritebatchDrawer bunnyDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BunnyStormBunny>()], NPC.Center);
        bunnyDrawer.VerticalFrame(0, 6);
        bunnyDrawer.CenterOrigin();

        for (int i = 0; i < _vortexPS.particles.Length; i++)
        {
            ref Vector2 pos = ref _vortexPS.particles.positions[i];
            bunnyDrawer.worldPosition = NPC.Center + pos;
            bunnyDrawer.rotation = Main.GlobalTimeWrappedHourly * 4 + i * 8;
            spriteBatch.Draw(bunnyDrawer);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        _bunnyNoiseTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Noise");
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        _earTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Ear");
        _whiskersTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Whiskers");

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, NPC.Center);
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly * 8;
        spiralVortexDrawer.color = Color.White * 0.12f;
        spiralVortexDrawer.color.A = 0;
        spriteBatch.Draw(spiralVortexDrawer);
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];

            SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, pos + NPC.Size * 0.5f);
            maskDrawer.VerticalFrame(_stormFrame, 4);
            maskDrawer.CenterOrigin();
            maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3);
            maskDrawer.rotation = _stormRotation;
            maskDrawer.color = Color.Lerp(Color.CadetBlue, Color.Black, i / (float)NPC.oldPos.Length) * 0.05f;
            maskDrawer.color.A = 0;
            spriteBatch.Draw(maskDrawer);
        }
        DrawCrystal(spriteBatch);



        float offset = 32;
        SpritebatchDrawer earDrawer = SpritebatchDrawer.FromTextureAsset(_earTextureAsset, NPC.Center);
        earDrawer.BottomLeftOrigin();
        earDrawer.drawOrigin.X += 32;
        earDrawer.drawOrigin.Y -= 16;
        earDrawer.worldPosition.X += offset;
        earDrawer.worldPosition.Y -= 64;
        earDrawer.worldPosition += -NPC.velocity * 4;
        earDrawer.rotation = NPC.velocity.X * 0.05f + ExtraMath.Osc(-0.3f, 0.3f);
        earDrawer.scale *= _stormScale;
        spriteBatch.Draw(earDrawer);


        earDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        earDrawer.drawOrigin.X = earDrawer.texture.Width - earDrawer.drawOrigin.X;
        earDrawer.worldPosition.X -= offset * 2;
        earDrawer.rotation = NPC.velocity.X * -0.05f + ExtraMath.Osc(0.3f, -0.3f, offset: 1);
        spriteBatch.Draw(earDrawer);



        RenderBunnyParticles(spriteBatch, screenPos, drawColor);


        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        glowDrawer.scale *= 0.3f;
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.2f, 0.6f, speed: 3);
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer whiskersDrawer = SpritebatchDrawer.FromTextureAsset(_whiskersTextureAsset, NPC.Center);
        whiskersDrawer.CenterOrigin();
        whiskersDrawer.rotation = NPC.velocity.X * 0.05f + ExtraMath.Osc(-0.3f, 0.3f);
        whiskersDrawer.scale *= _stormScale;
        spriteBatch.Draw(whiskersDrawer);


        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail, DrawLayer.BehindNPCsWithOutline);

        PixelationManager.QueueSpritebatchDrawAction(DrawGlow);
        OutlineRenderer.Queue(DrawBunnyParticles);
        /*
        drawer.color = Color.LightGreen * ExtraMath.Osc(0.5f, 1f, speed: 16);
        drawer.color.A = 0;
        drawer.VerticalFrame(2, 3);
        spriteBatch.Draw(drawer);
        */
        return false;
    }

    private void DrawBunnyMask(SpriteBatch sb)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        DrawMask(sb, 0);
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.BunnyStorm);
    }
}
