using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;


public class DeadRomanceParryBuster : ModProjectile
{

    private bool _noHoming;
    private bool _homeToOwner;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private int Target
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    private bool Bouncy => Projectile.ai[2] > 0;
    private float _targetScale;
    public bool reflect;
    public float hitstopTimer;
    public float reflectCount;
    public float hitCount;
    public float killTimer;
    public bool kill;
    public Vector2 squishScale;
    public float hitstopTime => 15;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition() && hitstopTimer <= 0;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(hitstopTimer);
        writer.Write(reflect);
        writer.Write(_noHoming);
        writer.Write(_homeToOwner);
        writer.Write(reflectCount);
        writer.Write(killTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        hitstopTimer = reader.ReadSingle();
        reflect = reader.ReadBoolean();
        _noHoming = reader.ReadBoolean();
        _homeToOwner = reader.ReadBoolean();
        reflectCount = reader.ReadSingle();
        killTimer = reader.ReadSingle();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 360;
        Projectile.light = 0.78f;
        Projectile.friendly = true;

    }


    public override void AI()
    {
        base.AI();
        if (hitstopTimer > 0)
        {
            hitstopTimer--;
            float ease = EasingFunction.InOutSine(hitstopTimer / hitstopTime);
            squishScale = Vector2.Lerp(Vector2.One, Vector2.One * 1.5f, ease);
        }
        else
        {
            float scaleMult = MathHelper.Lerp(1f, 1.75f, reflectCount / 5f);
            scaleMult *= MathHelper.Lerp(1f, 1.75f, hitCount / 5f);
            _targetScale = MathHelper.Lerp(_targetScale, scaleMult, 0.1f);
            squishScale = Vector2.One * _targetScale;
        }
        float speedMult = MathHelper.Lerp(1f, 2f, reflectCount / 5f);

        Timer++;
        if (Timer % 8 == 0)
        {
            SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero);
            sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.8f;
            sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        }
        if (Timer % 5 == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawnPos = Projectile.Center;
                spawnPos += Main.rand.NextVector2Circular(32, 32);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }

        }
        if (Owner.HasBuff<HeavenlyLove>())
            Projectile.ai[2] = 1;
        Projectile.friendly = !_homeToOwner;
        if (kill)
        {
            Projectile.scale *= 0.8f;
            if (Projectile.scale < 0.1f)
            {
                Projectile.Kill();
            }
            return;
        }
        if (_homeToOwner)
        {
            Vector2 targetVelocity = (Owner.Center - Projectile.Center);
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            targetVelocity *= 12 * speedMult;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
            DeadRomancePlayer romancePlayer = Owner.GetModPlayer<DeadRomancePlayer>();

            float dist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (dist < 64)
            {
                killTimer++;
                if (killTimer > 14)
                {
                    kill = true;
                }
            }
            if (reflect)
            {
                PlayGrowSound();
                killTimer = 0;
                reflectCount++;
                hitstopTimer = hitstopTime;
                _homeToOwner = false;
                _noHoming = false;
                reflect = false;
            }
        }
        else if (Target != -1)
        {
            reflect = false;
            if (!_noHoming)
            {
                NPC target = Main.npc[Target];
                if (!target.active)
                    Target = -1;
                Vector2 targetVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                float speed = MathHelper.Lerp(0.2f, 24, EasingFunction.InOutExpo(Timer / 60f));
                targetVelocity *= speed * speedMult; ;
                Vector2 lerpedVelocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
                Projectile.velocity = lerpedVelocity;
            }

        }
        if (Projectile.velocity.Length() < 25f)
            Projectile.velocity *= 1.05f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void PlayGrowSound()
    {
        SoundStyle growSound = AssetRegistry.Sounds.Melee.WeaponSwordbigger;
        growSound.Pitch = MathHelper.Lerp(0f, 0.8f, reflectCount / 5f);
        SoundEngine.PlaySound(growSound, Projectile.Center);
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        modifiers.FinalDamage *= MathHelper.Lerp(1f, 4f, reflectCount / 5f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Bouncy)
        {
            hitstopTimer = hitstopTime;
            _noHoming = true;
            _homeToOwner = true;
            Target = target.whoAmI;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 12);
            hitCount++;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero,
             ModContent.ProjectileType<HolyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            PlayGrowSound();
        }
        else
        {
            _noHoming = true;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero,
          ModContent.ProjectileType<HeavenlyCrashBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.Kill();
        }

        if (reflectCount >= 5)
        {
            reflectCount = 0;
            for (float f = 0; f < 8; f++)
            {
                float ratio = f / 8f;
                float radians = ratio * MathHelper.TwoPi;
                Vector2 vel = radians.ToRotationVector2() * 8;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel,
                    ModContent.ProjectileType<DeadRomanceBusterSmiteBlade>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<HeavenlyCrashBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.Kill();
        }

        var boom = FXUtil.GlowCircleBoom(target.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
        boom.Scale *= 2;
        for (float f = 0f; f < 8f; f++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
            Vector2 pos = target.Center;
            var ds = DustParticle.Spawn(pos, vel);
            ds.noTileCollide = true;
            ds.outerColor = Color.Yellow;
        }
        for (float f = 0; f < 4f; f++)
        {
            Vector2 pos = target.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 velocity = (pos - target.Center).SafeNormalize(Vector2.Zero) * 32;
            var fx = FXUtil.GlowStretch(pos, velocity);
            fx.OuterGlowColor = Color.Goldenrod;
        }

        PixelPrimitiveCircleFactory.CreateHeavenlyBoom(target.Center);
        SoundStyle busted = AssetRegistry.Sounds.Melee.ExcaliburHitBuster;
        SoundEngine.PlaySound(busted, target.Center);
    }

    private void DrawPixelatedAura(SpriteBatch sb, Vector2 sp)
    {
        float rotation = Projectile.rotation;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 worldPos = pos + Projectile.Size * 0.5f;
            drawer.worldPosition = worldPos;
            drawer.rotation = Projectile.oldRot[i];
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(ratio);
            Color bladeColor = Color.Lerp(Color.Goldenrod, Color.Black, ease);
            bladeColor.A = 0;
            drawer.color = bladeColor;
            sb.Draw(drawer);
        }


        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BeamTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.DirnTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.3f;
        sb.Restart(effect: shader.Effect);
        SpritebatchDrawer glowSwordSprite = SpritebatchDrawer.FromProjectile(Projectile);
        glowSwordSprite.rotation = rotation;
        glowSwordSprite.blackIsTransparency = true;
        glowSwordSprite.color = Color.White;
        glowSwordSprite.scale = new Vector2(1f, 1f) * squishScale;
        sb.Draw(glowSwordSprite);

        //        glowSwordSprite.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 4) * 12;
        glowSwordSprite.color = Color.Goldenrod;
        glowSwordSprite.scale *= 1.2f;
        glowSwordSprite.color *= 0.5f;
        sb.Draw(glowSwordSprite);
        sb.RestartDefaults();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedAura);
        return false;
    }
}
public class DeadRomanceParryingBlade : ModProjectile
{

    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 24;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.78f;
        Projectile.hide = true;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        overPlayers.Add(index);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Owner.GetModPlayer<DeadRomancePlayer>().StartParry();
            FXUtil.ShakeCamera(Projectile.Center, 1024, 2);
            SoundStyle parrySound = AssetRegistry.Sounds.Melee.ExcaliburParry;
            parrySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(parrySound, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                var sp = SparkleParticle.Spawn(Projectile.Center, velocity);
                sp.outerColor = Color.Goldenrod;
                sp.Scale *= 0.6f;
                sp.noTileCollide = true;
                sp.gravity = 0;
            }
        }

        Projectile.Center = Owner.Center;
        AI_OrientPlayer();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> projTexture = TextureAssets.Item[Owner.HeldItem.ModItem.Type];
        SpritebatchDrawer parryDrawer = SpritebatchDrawer.FromTextureAsset(projTexture, Projectile.Center);
        parryDrawer.rotation = MathHelper.ToRadians(90);
        parryDrawer.spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None; ;
        if (Projectile.spriteDirection == -1)
        {
            parryDrawer.rotation += MathHelper.TwoPi + MathHelper.ToRadians(180);
        }
        Main.spriteBatch.Draw(parryDrawer);


        float ratio = (float)Projectile.timeLeft / 24f;
        for (int i = 0; i < 8; i++)
        {
            parryDrawer.color = Color.White * ratio;
            parryDrawer.color.A = 0;
            Main.spriteBatch.Draw(parryDrawer);
        }
        return false;
    }
    private void AI_OrientPlayer()
    {
        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
