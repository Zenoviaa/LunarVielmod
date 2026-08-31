using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.RarityRendering;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.GothinFlames;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Projectiles.Steins;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class Gothinstein : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 142;
        Item.useTime = 7;
        Item.useAnimation = 7;
        Item.shoot = ModContent.ProjectileType<GothinsteinBarrage>();
        Item.rare = ModContent.RarityType<NoHitRarity>();
        staminaProjectileShoot = ModContent.ProjectileType<GothFist>();
        meleeWeaponType = MeleeWeaponType.Stein;
        staminaDamageMultiplier = 2;
        staminaCost = 3;
    }
    public override void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int style = 0;
        GothinsteinComboPlayer comboPlayer = player.GetModPlayer<GothinsteinComboPlayer>();
        if (comboPlayer.comboCounter >= 10)
        {
            style = 1;
            comboPlayer.comboCounter = 0;
            comboPlayer.direction++;
            comboPlayer.direction %= 2;
            damage *= 2;
        }
        int dir = comboPlayer.direction % 2 == 0 ? -1 : 1;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: style , ai2: dir);
    }

    protected override void ShootStaminaProj(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        base.ShootStaminaProj(player, source, position, velocity, type, damage, knockback);
    }
}

public class GothinsteinComboPlayer : ModPlayer
{
    public int comboCounter;
    public int direction;
}

public class GothinsteinBarrage : ModProjectile
{
    private Vector2 _start;
    private Vector2 _end;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    private ref float Direction => ref Projectile.ai[2];
    private float HitStopTimer;
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DefaultToSteinFistProjectile();
        Projectile.timeLeft = 32;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_start);
        writer.WriteVector2(_end);
        writer.Write(HitStopTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _start = reader.ReadVector2();
        _end = reader.ReadVector2();
        HitStopTimer = reader.ReadSingle();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override void AI()
    {
        base.AI();
        if(HitStopTimer > 0)
        {
            HitStopTimer--;
            Timer--;
            Projectile.timeLeft++;
        }

        Timer++;
        if (Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                _start = Owner.Center + Main.rand.NextVector2Circular(45, 45);
                ProjFirer firer = ProjFirer.From<GothinsteinFlameWave>(Projectile);
                firer.position = _start;
                firer.damage /= 3;
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 16;
                firer.New();

                if (Style == 1)
                {
                    var sound = AssetRegistry.Sounds.Fire.FlaminChargeFast;
                    SoundEngine.PlaySound(sound with { PitchVariance = 0.3f }, Projectile.position);
                    _start = Owner.Center;
                    _end = MovementUtilities.SteinGetEndPoint(Owner, _start, Main.MouseWorld, 212);
                }
                else
                {
                    _end = MovementUtilities.SteinGetEndPoint(Owner, _start, Main.MouseWorld, 180);
                }
             
                if (Style == 0)
                {
                    NormalVector2 direction = new((_end - _start).RotatedBy(MathHelper.PiOver2));
                    _start += direction * (Main.rand.NextBool(2) ? -36 : 36);
                }
      

                Projectile.netUpdate = true;
            }
        }
    
        if (Timer == 2)
        {

            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
            ThrustParticle ts = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity);
            ts.bloomColor = Color.Teal;
            ts.Scale *= 0.5f;

            for (int j = 0; j < 2; j++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(8, 32);
                Color color = Color.Lerp(Color.White, Color.Aqua, Main.rand.NextFloat(0f, 1f));
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = Main.rand.Next(45, 90),
                    innerColor = color.ToVector4(),
                    outerColor = Color.Cyan.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.2f, 0.5f))
                });
            }
        }

        int denom = 6;
        if (Main.rand.NextBool(denom))
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = pos,
                velocity = -Projectile.velocity * 0.47f,
                timeLeft = 45,
                innerColor = color.ToVector4(),
                outerColor = Color.Red.ToVector4()
            });
        }
        if (Timer % 8 == 0)
        {
            var ts = ThickSmokeParticle.Spawn(Projectile.Center, Vector2.Zero);
            ts.expand = true;
            ts.color *= 0.5f;
            ts.Scale *= 0.2f;
        }

        switch (Style)
        {
            case 0:
                {
                    Projectile.Center = MovementUtilities.SteinCalculateSwingPoint( Timer / 16f, _start, _end);
                    Projectile.rotation = (_end - _start).ToRotation();
                    if(Timer >= 16)
                    {
                        Projectile.Kill();
                    }
                }
                break;
            case 1:
                {

                    //Calculate positions to travel to
                    SteinUppercutParameters parameters = new()
                    {
                        start = _start,
                        end = _end,
                        direction = new Vector2(1, Direction),
                        ratio = Timer / 32f,
                        swingRadians = MathHelper.Pi,
                        rotation = Projectile.velocity.ToRotation(),
                        ySize = 256
                    };
                    Vector2 upperCointPoint = MovementUtilities.SteinCalculateUppercutSwingPoint( parameters);
                    parameters.ratio += 0.05f;
                    Vector2 p2 = MovementUtilities.SteinCalculateUppercutSwingPoint( parameters);


                    //Travel to those points
                    Projectile.Center = upperCointPoint;
                    Projectile.rotation = (p2 - upperCointPoint).ToRotation();
                    if (Timer >= 32)
                    {
                        Projectile.Kill();
                    }
                }
                break;
        }

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
      
        if(Style == 1)
        {
            var impact = ProjFirer.From<GothinsteinImpact>(Projectile);
            impact.velocity = Vector2.Zero;
            impact.New();
            HitStopTimer = 4;
        }
        else
        {
            Owner.GetModPlayer<GothinsteinComboPlayer>().comboCounter++;
        }
    }
    private void DrawFlameTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(96, 64, ratio) * 0.25f * MathHelper.SmoothStep(1f, 0f, EasingFunction.InExpo(Timer / 32f));
        }
        float GetTrailWidth2(float ratio)
        {
            return GetTrailWidth(ratio) * 2f;
        }
        Color GetTrailColor(float ratio)
        {
            return DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.White, Color.OrangeRed, Color.Red, Color.DarkRed, Color.Black);
            //    return Color.Lerp(Color.Lerp(Color.White, Color.Yellow, EasingFunction.OutQuad(ratio)), Color.Lerp(Color.Orange, Color.Lerp(Color.Red, Color.Transparent, ratio), EasingFunction.OutQuad(ratio)), EasingFunction.OutExpo(ratio)) * _afterImageAlpha;
        }

        Color GetTrailColor2(float ratio)
        {
            return Color.Lerp(GetTrailColor(ratio), Color.DarkRed, 0.5f) * 0.3f * MathHelper.Lerp(1f, 0f, ratio) * 4;
        }

        GothinFlameTrailShader flameTrailShader = ShaderContent.GetInstance<GothinFlameTrailShader>();
        flameTrailShader.InsideColor = Color.Gold;
        flameTrailShader.BloomColor = Color.Red;
        flameTrailShader.TransformMatrix = TrailDrawer.WorldViewPoint2;


        flameTrailShader.LaserTexture = AssetManager.LaserTextures.FlameTrail.Value;
        flameTrailShader.Time = Main.GlobalTimeWrappedHourly * 24;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor, GetTrailWidth, flameTrailShader, Projectile.Size * 0.5f);

        flameTrailShader.LaserTexture = TrailRegistry.WhispyTrail.Value;
        TrailDrawer.Draw(Projectile.oldPos, GetTrailColor2, GetTrailWidth2, flameTrailShader, Projectile.Size * 0.5f);
    }

    private void DrawGlow()
    {
        Vector2 scale = new Vector2(0.6f, 0.5f);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        drawer.color = Color.OrangeRed * 0.5f;
        drawer.color.A = 0;
        drawer.scale *= 0.6f * scale;
        drawer.scale.X *= 0.74f;
        drawer.scale.Y *= 0.8f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.Gold * 0.5f;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);

        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 0.84f;
        drawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawGlow();
        SpritebatchDrawer fadeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float fadeOut = MathHelper.SmoothStep(0f, 1f, Projectile.timeLeft / 12f);
        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            fadeDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            fadeDrawer.color = Color.Lerp(Color.White, Color.Transparent, oldPos.progress) * fadeOut * 0.3f;
            fadeDrawer.rotation = Projectile.oldRot[oldPos.index];
            Main.spriteBatch.Draw(fadeDrawer);
        }
        var spriteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(spriteDrawer);
        SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        outlineDrawer.VerticalFrame(1, 2);
        if(Style == 0)
        {
            outlineDrawer.color = Color.Lerp(Color.Aqua, Color.Teal, ExtraMath.Osc(0f, 1f, speed: 12)) * fadeOut;
        }
        else
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawFlameTrail, DrawLayer.OverNPCsAdditive);
            outlineDrawer.color = Color.Lerp(Color.Orange, Color.OrangeRed, ExtraMath.Osc(0f, 1f, speed: 12)) * fadeOut;
        }

        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }
}

public class GothinsteinFlameWave : ModProjectile, IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Hit => ref Projectile.ai[1];
    private ref float Scale => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 80;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 2;

    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                Scale = Main.rand.NextFloat(0.6f, 1f);
                Projectile.netUpdate = true;
            }
        }

        if(Hit > 0)
        {
            Projectile.extraUpdates = 4;
        }

        int denom = 16;
        if (Main.rand.NextBool(denom))
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(16, 16);
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = pos,
                velocity =Main.rand.NextVector2Circular(12, 12),
                timeLeft = 45,
                innerColor = color.ToVector4(),
                outerColor = Color.Red.ToVector4()
            });
        }
        Projectile.velocity *= 0.97f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.scale = Scale;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Hit = 1;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    private void DrawFlameBow(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        float dissipate = EasingFunction.InOutSine(Projectile.timeLeft / 60f);
        FlameBowShader flamebowShader = ShaderContent.GetInstance<FlameBowShader>();
        flamebowShader.Time = Main.GlobalTimeWrappedHourly * -24;
        flamebowShader.FlameNoiseTexture = AssetManager.Noise.InvertedVoronoi;
        flamebowShader.InsideColor = Color.Yellow;
        flamebowShader.BloomColor = Color.Red;
        flamebowShader.DissipateThreshold = 0f;
        flamebowShader.DistortionStrength = 0.05f;
        float alpha = EasingFunction.InOutSine(Projectile.timeLeft / 80f);
        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { effect = flamebowShader.Effect }))
        {
            SpritebatchDrawer projBowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            projBowDrawer.scale *= 0.75f;
            projBowDrawer.rotation = Projectile.velocity.ToRotation();

            projBowDrawer.CenterOrigin();

            foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
            {
             
                projBowDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
                projBowDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 18) * alpha * 0.85f * MathHelper.Lerp(1f, 0f, oldPos.progress) * alpha;
                projBowDrawer.color.A = 0;
            
                spriteBatch.Draw(projBowDrawer);
            }
            SpritebatchDrawer bowDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            bowDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 18) * alpha * 4;
            bowDrawer.color.A = 0;
            bowDrawer.rotation = Projectile.velocity.ToRotation();
            bowDrawer.CenterOrigin();
            bowDrawer.scale *= 0.75f;
            spriteBatch.Draw(bowDrawer);
  
            bowDrawer.color = Color.DarkRed * ExtraMath.Osc(0.8f, 1f, speed: 12) * 0.35f * alpha;
            bowDrawer.color.A = 0;
            bowDrawer.scale *= 1.2f;

            spriteBatch.Draw(bowDrawer);
        }

        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.45f * alpha;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.5f;
        spriteBatch.Draw(glowDrawer);
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawFlameBow);
    }
}

public class GothinsteinImpact : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private float Time => 24;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Time;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetRegistry.Sounds.Fire.FireExplosion1;
            SoundEngine.PlaySound(sound with { PitchVariance = 0.3f }, Projectile.position);
            ShakeScreenPosition.Shake = 4;
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.LightGoldenrodYellow, Color.OrangeRed, 18, 128);

            for (int j = 0; j < 16; j++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = Main.rand.NextVector2Circular(32, 32);
                Color color = Color.Lerp(Color.White, Color.Yellow, Main.rand.NextFloat(0f, 1f));
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = Main.rand.Next(45, 90),
                    innerColor = color.ToVector4(),
                    outerColor = Color.Red.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.2f, 1f))
                });
            }
        }

    }

    private void DrawPixelatedBoom(SpriteBatch sb, Vector2 screenPos)
    {
        Asset<Texture2D> noiseTextureAsset = AssetManager.Noise.FlamethrowerNoise;
        FlameyBoomShader boomShader = ShaderContent.GetInstance<FlameyBoomShader>();
        float t = Timer / Time;
        boomShader.NoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        boomShader.Time = EasingFunction.OutSine(t);
        boomShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, t);
        boomShader.BloomColor = Color.Lerp(Color.Red, Color.DarkRed, t);
        float scale = 0.2f;
        sb.Restart(effect: boomShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.InvertedVoronoi.Asset.Value, Projectile.Center);
        drawer.color = Color.White;

        drawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 1, EasingFunction.OutQuad(t)) * 1.5f * scale;
        sb.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.FlamethrowerNoise, Projectile.Center);
        drawer.color = Color.White;

        drawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 1, EasingFunction.OutQuad(t)) * 12 * scale;
        sb.Draw(drawer);
        sb.RestartDefaults();
    }
    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBoom, DrawLayer.OverNPCsAdditive);
    }
}

public class GothFist : ModProjectile
{
    private Vector2 _originalPosition;
    public override string Texture => TextureRegistry.EmptyTexture;
    public float Timer
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public bool Bounced
    {
        get
        {
            return Projectile.ai[1] == 1;
        }
        set
        {
            Projectile.ai[1] = value ? 1 : 0;
        }
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }

    public override void SetDefaults()
    {
        Projectile.DefaultToSteinProjectile();
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            _originalPosition = Projectile.Center;
        }

        AttachToPlayer();
    }

    public void AttachToPlayer()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.active || player.dead || player.CCed || player.noItems)
            return;
        Vector2 teleportPosition = Main.MouseWorld;
        if (Timer == 5 && Main.myPlayer == Projectile.owner)
        {
            SteinHelper.SteinDash(player, Projectile, teleportPosition);
        }

        Projectile.velocity *= 0.97f;
        Vector2 oldMouseWorld = Main.MouseWorld;
        if (Timer > 8)
        {

            if (Timer < 10 && Main.myPlayer == Projectile.owner)
            {

                player.velocity = Projectile.DirectionTo(oldMouseWorld) * 13f;
            }
        }
        if (Timer == 8)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Vector2.Lerp(_originalPosition, Projectile.Center, Main.rand.NextFloat(0f, 1f));
                pos += Main.rand.NextVector2Circular(32, 32);
                var fx = FXUtil.GlowStretch(pos, Projectile.velocity.SafeNormalize(Vector2.Zero) * 32);
                fx.OuterGlowColor = Color.Gold;
            }

            for (int j = 0; j < 32; j++)
            {
                Vector2 pos = Vector2.Lerp(_originalPosition, Projectile.Center, Main.rand.NextFloat(0f, 1f));
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(8, 32);
                Color color = Color.Lerp(Color.White, Color.Aqua, Main.rand.NextFloat(0f, 1f));
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = Main.rand.Next(45, 90),
                    innerColor = color.ToVector4(),
                    outerColor = Color.Cyan.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.8f, 1.6f))
                });
            }
        }

        if (Timer == 25)
        {
            if (!Bounced)
            {
                player.itemTime = 155;
                player.itemAnimation = 155;
            }
            if (Bounced)
            {
                player.itemTime = 60;
                player.itemAnimation = 60;
            }
        }
    }

    public override bool? CanDamage()
    {

        if (Timer < 8)
        {
            return false;
        }

        return base.CanDamage();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[Projectile.owner];
        Vector2 oldMouseWorld = Main.MouseWorld;
        player.GetModPlayer<SteinPlayer>().HasHitDance = true;
        if (!Bounced)
        {
            PixelPrimitiveCircleFactory.CreateGothInwardBoom(target.Center);
            player.GetModPlayer<DashPlayer>().DashCount += 3;
            player.velocity = Projectile.DirectionTo(oldMouseWorld) * -17f;
            Bounced = true;

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinGoth") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
            switch (Main.rand.Next(7))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice1") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice2") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;
                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice3") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;

                case 3:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinIk") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;

                case 4:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinHulting") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;

                case 5:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinShading") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;

                case 6:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinVolting") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });

                    break;


            }





            switch (Main.rand.Next(3))
            {
                case 0:

                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit1"), Projectile.Center);
                    break;
                case 1:

                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit2"), Projectile.Center);
                    break;
                case 2:

                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Steinhit3"), Projectile.Center);
                    break;

            }

            //Wow, Amazing, So Hot, SEXY, Great
            switch (Main.rand.Next(11))
            {
                case 0:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), Projectile.damage * 3, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    break;
                case 1:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), Projectile.damage * 3, 0f, Projectile.owner, 0f, 0f);
                    break;
                case 2:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), Projectile.damage * 5, 0f, Projectile.owner, 0f, 0f);
                    break;
                case 3:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), Projectile.damage * 4, 0f, Projectile.owner, 0f, 0f);
                    break;
                case 4:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    break;
                case 5:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), Projectile.damage * 3, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    break;

                case 6:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), Projectile.damage * 4, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    break;

                case 7:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<GREAT>(), Projectile.damage * 5, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<AMAZING>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SEXY>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    break;

                case 8:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SEXY>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                    break;

                case 9:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<SOHOT>(), Projectile.damage * 4, 0f, Projectile.owner, 0f, 0f);
                    break;

                case 10:
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<WOW>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                    break;


            }
            float rot = player.velocity.ToRotation();
            float spread = 0.6f;

            Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);
                Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 180, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.SpringGreen * 0.5f, Main.rand.NextFloat(0.5f, 1));

            }




            switch (Main.rand.Next(12))
            {
                case 0:
                    target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.ForestGreen, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.GreenYellow, 1f).noGravity = true;
                    }



                    break;
                case 1:

                    target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 46; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.ForestGreen, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                    }
                    break;
                case 2:
                    target.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Freidhit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 66; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.ForestGreen, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.LightGoldenrodYellow, 1f).noGravity = true;
                    }
                    break;

                case 3:
                    target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Hulthit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Pink, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DeepPink, 1f).noGravity = true;
                    }



                    break;

                case 4:
                    target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Hulthit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.White, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.IndianRed, 1f).noGravity = true;
                    }
                    break;


                case 5:
                    target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Ikhit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Blue, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
                    }



                    break;
                case 6:

                    target.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Ikhit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Blue, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
                    }
                    break;
                case 7:
                    target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Ikhit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Blue, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DarkRed, 1f).noGravity = true;
                    }
                    break;

                case 8:
                    target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }


                    break;
                case 9:

                    target.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Purple, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Purple, 1f).noGravity = true;
                    }
                    break;
                case 10:
                    target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 15; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }
                    break;

                case 11:
                    target.SimpleStrikeNPC(Projectile.damage * 20, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Shit4>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 35; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 1f).noGravity = true;
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Black, 0.5f).noGravity = true;
                    }
                    break;

                case 12:
                    target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Volthit1>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);


                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Yellow, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Gold, 1f).noGravity = true;
                    }



                    break;
                case 13:

                    target.SimpleStrikeNPC(Projectile.damage * 10, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Volthit2>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Yellow, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Gold, 1f).noGravity = true;
                    }
                    break;
                case 14:
                    target.SimpleStrikeNPC(Projectile.damage * 15, 1, crit: false, Projectile.knockBack);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0, ModContent.ProjectileType<Volthit3>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
                    for (int i = 0; i < 26; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Yellow, 1f).noGravity = true;
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.Gold, 1f).noGravity = true;
                    }
                    break;

            }

            target.SimpleStrikeNPC(Projectile.damage * 5, 1, crit: false, 1);
        }
    }

    public float WidthFunction(float completionRatio)
    {
        return 124 * MathHelper.SmoothStep(1f, 0f, EasingFunction.OutSine(Timer / (float)60f));
    }

    private Color GetColorFunction(float completionRatio)
    {
        float inRatio = completionRatio / 0.3f;
        inRatio = EasingFunction.InOutSine(inRatio);
        float outRatio = (1f - completionRatio) / 0.3f;
        outRatio = EasingFunction.InOutSine(outRatio);
        return Color.Yellow * inRatio * outRatio;
    }


    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        FixedRichLaserShader shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        Vector2[] array = new Vector2[64];
        for (int i = 0; i < array.Length; i++)
        {
            float ratio = i / (float)array.Length;
            ref Vector2 point = ref array[i];
            point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
        }
        shader.OuterColor = Color.OrangeRed;
        shader.LaserTexture = AssetRegistry.LaserTextures.TexturedLaser2;
        TrailDrawer.Draw(Main.spriteBatch, array, GetColorFunction, WidthFunction, shader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);
        return false;
    }
}