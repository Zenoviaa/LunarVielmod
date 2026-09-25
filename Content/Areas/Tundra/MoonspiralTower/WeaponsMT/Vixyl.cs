using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class VixylPlayer : ModPlayer
{
    public int parryCooldown;
    public int parryTimer;
    public bool hitParry;

    public override void PostUpdateEquips()
    {
        parryTimer--;
        if (parryTimer <= 0)
        {
            parryTimer = 0;
        }

        parryCooldown--;
        if (parryCooldown <= 0)
        {
            parryCooldown = 0;
        }
    }

    public override bool ConsumableDodge(Player.HurtInfo info)
    {
        if (parryTimer > 0 && parryCooldown <= 0)
        {
            parryTimer = 0;
            ParryEffects();
            return true;
        }

        return false;
    }
    public void StartParry()
    {
        hitParry = false;
        parryTimer = 4;
    }

    public void ParryEffects()
    {
        //Brief invulnerability after parrying
        // Some sound and visual effects
        for (int i = 0; i < 50; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
            Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.BlueCrystalShard, speed * 5, Scale: 1.5f);
            d.noGravity = true;
        }
        SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f }, Player.position);

        //Spawn the big verlia slash projectile here
        //Setting the immune time
        Player.SetImmuneTimeForAllTypes(60);
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }

        // Add the buff and assigning the cooldown time
        int time = 180;
        Player.AddBuff(ModContent.BuffType<VixylDodgeBuff>(), time);

        Vector2 velocity = Player.Center.DirectionTo(Main.MouseWorld);
        Projectile.NewProjectile(Player.GetSource_FromThis(),
            Player.Center, velocity, ModContent.ProjectileType<VixylParryProj>(), Player.HeldItem.damage * 4, Player.HeldItem.knockBack, Player.whoAmI);
        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"), Player.position);

        parryCooldown = time;
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            SendExampleDodgeMessage(Player.whoAmI);
        }
    }

    public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI)
    {
        int player = reader.ReadByte();
        if (Main.netMode == NetmodeID.Server)
        {
            player = whoAmI;
        }

        VixylPlayer vixylPlayer = Main.player[player].GetModPlayer<VixylPlayer>();
        vixylPlayer.ParryEffects();

        if (Main.netMode == NetmodeID.Server)
        {
            // If the server receives this message, it sends it to all other clients to sync the effects.
            SendExampleDodgeMessage(player);
        }
    }

    public static void SendExampleDodgeMessage(int whoAmI)
    {
        // This code is called by both the initial 
        ModPacket packet = ModContent.GetInstance<Stellamod>().GetPacket();
        packet.Write((byte)MessageType.Dodge);
        packet.Write((byte)whoAmI);
        packet.Send(ignoreClient: whoAmI);
    }
}

public class Vixyl : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 23;
        Item.shoot = ModContent.ProjectileType<ViyxlSwordSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<VixylParryingBlade>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 1;
    }


    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
}

public class VixylParryingBlade : ModProjectile
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
            Owner.GetModPlayer<VixylPlayer>().StartParry();
            SoundStyle parrySound = AssetReferences.Assets.Sounds.SwordSheethe.Asset with { PitchVariance = 0.75f };
            parrySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(parrySound, Projectile.position);

            for (int i = 0; i < 24; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8, 8);
                var sp = SparkleParticle.Spawn(Projectile.Center, velocity);
                sp.outerColor = Color.DarkBlue;
                sp.innerColor = Color.SkyBlue;
                sp.Scale *= 0.4f;
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.dampening = 0.05f;
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


        float ratio = Projectile.timeLeft / 24f;
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

public class ViyxlSwordSlash : BaseSwingProjectileV2
{
    private bool _hit;
    private bool _playedSound;
    private float _traveledRotation;
    private float _oldRot;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddSwordSwingStyleVixyl(this);
        additive = true;
        useAfterImage = true;

        trailOffsetOverride = 1.27f;
        outlineColor = Color.SkyBlue;
    }
    private Color GetTrailColor(float ratio)
    {
        Color c = Color.Lerp(Color.White,
            Color.Lerp(Color.SkyBlue, Color.Blue, ExtraMath.Osc(0f, 0.5f, speed: 8)), ratio)
            * 1f;// * EasingFunction.QuadraticBump(_swingTrailAlpha);
        c *= MathHelper.Lerp(0f, 1f, ratio);                                                                                                                                                                   // c.A = 0;
        return c;
    }
    private Color GetTrailColor2(float ratio)
    {
        Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 16)), ratio) * 0.24f * EasingFunction.QuadraticBump(ratio);// * EasingFunction.QuadraticBump(_swingTrailAlpha);                                                                                                                     // c.A = 0;
        c *= MathHelper.Lerp(0f, 1f, ratio);
        return c;
    }
    private Color GetTrailColor3(float ratio)
    {
        Color c = Color.White;                                                                                                                 // c.A = 0;
        c *= MathHelper.Lerp(0f, 1f, ratio) * 1.05f;
        return c;
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.Lerp(0, 24, ratio);
    }
    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.5f;
    }
    private float GetTrailWidth3(float ratio)
    {
        return GetTrailWidth(ratio) * 2f;
    }
    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        //Own custom trailing :o
        //FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Blue;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, shader);
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth2, shader);

        FixedRichLaserShader shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserTexture = TrailRegistry.BeamTrail;
        TrailDrawer.Draw(points, GetTrailColor2, GetTrailWidth3, shader2);



        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = Color.DarkBlue.ToVector3();
        pass.Parameters.bloomColor = Color.SkyBlue.ToVector3();
        //  pass.Parameters.time = Main.GlobalTimeWrappedHourly * 

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();

        TrailDrawer.Draw(swingTrailCache, GetTrailColor, GetTrailWidth, pass.Shader);
        TrailDrawer.Draw(swingTrailCache, GetTrailColor3, GetTrailWidth, pass.Shader);
    }

    public override void AI()
    {
        base.AI();
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        glowColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, EasingFunction.QuadraticBump(Interpolant));
        if (Timer % 16 == 0 && Interpolant >= 0.3f)
        {
            if (!_playedSound && IsFinishingSwing())
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    ShockCircleUpdater.Create(new()
                    {
                        position = Owner.Center,
                        time = 120,
                        startScale = 0.1f,
                        endScale = 1,
                        colorOverTime = (float f) => Color.Lerp(Color.White, Color.SkyBlue, f) * MathHelper.Lerp(1f, 0f, f),
                        easing = EasingFunction.OutExpo,
                        textureAsset = AssetReferences.Assets.NoiseTextures.BeamTrail.Asset
                    });
                }
                var hold = AssetReferences.Assets.Sounds.SwordHoldVerlia.Asset with { PitchVariance = 0.4f };
                SoundEngine.PlaySound(hold, Projectile.Center);
                _playedSound = true;
            }
        }

        _traveledRotation += MathF.Abs(Projectile.rotation - _oldRot);
        _oldRot = Projectile.rotation;

        if(IsFinishingSwing() && Timer % 16 == 0 && this.OwnedByLocalClient() && Interpolant > 0.5f)
        {
            var firer = ProjFirer.From<VixylFlyingSlash>(Projectile);
            firer.ai1 = 128 * Main.rand.NextFloat(0.5f, 2f);
            firer.ai2 = 48 * Main.rand.NextFloat(0.5f, 2f);

            firer.velocity = Main.rand.NextVector2Circular(4, 4);
            firer.position = Owner.Center + firer.velocity * 32;// Main.rand.NextVector2Circular(96, 96);
           
            firer.New();
        }

        if (_traveledRotation > 0.1f)
        {
            _traveledRotation = 0f;
            Vector2 spawnPos = swingTrailCache.InterpolateArrayClamped(Interpolant);
            if (Main.rand.NextBool(6))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = spawnPos,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    velocity = swingTrailCache.DirectionToNextPoint(Interpolant) * 4
                });
            }
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!_hit)
        {
            _hit = true;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = SoundRegistry.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (IsFinishingSwing())
        {
            DamageHelper.PercentIncreasedamage(ref modifiers, 0.5f);
        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class VixylFlyingSlash : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private float Time => 48;
    private ref float Timer => ref Projectile.ai[0];
    private ref float X => ref Projectile.ai[1];
    private ref float Y => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 1;
    }

    private Color GetTrailColor(float ratio)
    {
        Color c = Color.Lerp(Color.White,
            Color.Lerp(Color.SkyBlue, Color.Blue, ExtraMath.Osc(0f, 0.5f, speed: 8)), ratio)
            * 1f;
        c *= MathHelper.Lerp(0f, 1f, ratio);                                                                                                                                                                   // c.A = 0;
        return c;
    }
    private Color GetTrailColor3(float ratio)
    {
        Color c = Color.White;                                                                                                                 // c.A = 0;
        c *= EasingFunction.QuadraticBump(ratio) * 1.05f;
        return c;
    }
    private float GetTrailWidth(float ratio)
    {
        return 24 * EasingFunction.QuadraticBump(ratio) * MathHelper.SmoothStep(1f, 0f, Timer / Time);
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        var maxProgress = EasingFunction.OutExpo(Timer / Time);
        var points = MovementUtilities.SwingPoints(Projectile.Center, maxProgress, 64, MathHelper.ToRadians(270), X, Y, Projectile.velocity.ToRotation());
        var pass = AssetReferences.Effects.Abyss.SingularTrail.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.insideColor = Color.DarkBlue.ToVector3();
        pass.Parameters.bloomColor = Color.SkyBlue.ToVector3();
        //  pass.Parameters.time = Main.GlobalTimeWrappedHourly * 

        var hlsl = new HlslSampler();
        hlsl.Sampler = SamplerState.LinearWrap;
        hlsl.Texture = AssetReferences.Assets.LaserTextures.Beamlight.Asset.Value;
        pass.Parameters.laserSampler = hlsl;
        pass.Apply();


        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, pass.Shader);
        TrailDrawer.Draw(points, GetTrailColor3, GetTrailWidth, pass.Shader);
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 8)
        {
            for(var f = 0; f < 10; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    innerColor = Color.SkyBlue.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(8, 8)
                });
            }
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.OverWater);
    }
}

public class VixylSlashProj : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 10;
    }

    public override void SetDefaults()
    {
        Projectile.width = 300;
        Projectile.height = 300;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 36;
        Projectile.localNPCHitCooldown = 4;
        Projectile.usesLocalNPCImmunity = true;
    }

    float trueFrame = 0;
    public void UpdateFrame(float speed, int minFrame, int maxFrame)
    {
        trueFrame += speed;
        if (trueFrame < minFrame)
        {
            trueFrame = minFrame;
        }
        if (trueFrame > maxFrame)
        {
            trueFrame = minFrame;
        }
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            if (Main.netMode != NetmodeID.Server)
            {
                ShockCircleUpdater.Create(new()
                {
                    position = Projectile.Center,
                    time = 120,
                    startScale = 0.1f,
                    endScale = 1,
                    colorOverTime = (float f) => Color.Lerp(Color.White, Color.SkyBlue, f) * MathHelper.Lerp(1f, 0f, f),
                    easing = EasingFunction.OutExpo,
                    textureAsset = AssetReferences.Assets.NoiseTextures.BeamTrail.Asset
                });
            }
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 25, 252);
        }

        if (Timer == 1 && this.OwnedByLocalClient())
        {
            Projectile.velocity = (Main.MouseWorld - Projectile.Center);

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Projectile.velocity *= 7;
            Projectile.netUpdate = true;
        }
        if(Timer == 1)
        {
            var hold = AssetReferences.Assets.Sounds.SwordHoldVerlia.Asset with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(hold, Projectile.Center);
        }
        Projectile.velocity *= 0.96f;
        Main.projFrames[Type] = 10;
        Projectile.Center = Owner.Center;
        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 10)
            {
                Projectile.frame = 0;
            }
        }
        Owner.SetImmuneTimeForAllTypes(3);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var ratio = Timer / 36f;
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles.MoonSlashHold.Asset, Projectile.Center);
        sbDrawer.VerticalFrame(Projectile.frame, Main.projFrames[Type]);
        sbDrawer.CenterOrigin();
        sbDrawer.color = Color.White;
        sbDrawer.color.A = 0;
        sbDrawer.color *= MathHelper.SmoothStep(1f, 0f, EasingFunction.InSine(ratio));
        sbDrawer.scale *= MathHelper.SmoothStep(0.8f, 1.2f, ratio);
        Main.spriteBatch.Draw(sbDrawer);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
}

public class VixylParryProj : ModProjectile
{
    public float Timer
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Empress's Moon Slash");
        Main.projFrames[Projectile.type] = 7;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.width = 200;
        Projectile.height = 200;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 21;
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.scale = 1;
        DrawOffsetX = -100;
    }

    public override void AI()
    {

        Projectile.rotation = Projectile.velocity.ToRotation();
        Timer++;
        if (Timer == 2)
        {
            Projectile.scale *= 0.98f;
            Timer = 0;
        }
        if (Projectile.timeLeft == 4 && this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<VixylSlashProj>(Projectile);
            firer.New();
        }


        if (Projectile.scale == 0f)
        {
            Projectile.Kill();
        }

        //Visual Stuff
        Vector3 RGB = new(0.89f, 2.53f, 2.55f);
        // The multiplication here wasn't doing anything
        Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
    }

    public override bool PreAI()
    {
        Projectile.tileCollide = false;
        if (++Projectile.frameCounter >= 3)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 7)
            {
                Projectile.frame = 0;
            }
        }
        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
    }
}
