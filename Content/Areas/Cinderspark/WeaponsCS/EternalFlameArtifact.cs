using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

public class EternalFlameArtifact : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 18;
        Item.mana = 24;
        Item.width = 18;
        Item.height = 21;
        Item.useTime = Item.useAnimation = 15;

        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 4f;
        Item.DamageType = DamageClass.Magic;
        Item.value = 10000;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.DD2_BookStaffCast;
        Item.shoot = ModContent.ProjectileType<EternalFlameBall>();
        Item.shootSpeed = 4f;
        Item.autoReuse = true;
        Item.noMelee = true;
    }


    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        Item.useTime = Item.useAnimation = 15;
        Item.noUseGraphic = true;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        (p.ModProjectile as StaffWaveHold).MagicCircleStyle = 2;
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankStaff>();
    }
}


public class EternalFlameBall : ModProjectile
{
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;
    private Vector2 _initialPosition;
    private ref float Timer => ref Projectile.ai[0];
    private ref float EaseBackTimer => ref Projectile.ai[1];
    private ref float Rand => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startPosition);
        writer.WriteVector2(_initialPosition);
        writer.WriteVector2(_initialVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startPosition = reader.ReadVector2();
        _initialPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle fireSound;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1");
                    break;
                case 1:
                    fireSound = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot2");
                    break;
            }
            fireSound = fireSound with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(fireSound, Projectile.position);

            if (this.OwnedByLocalClient())
            {
                Rand = Main.rand.NextFloat(-1f, 1f);
                _initialPosition = Projectile.Center;
                _initialVelocity = Projectile.velocity;
                Projectile.netUpdate = true;
            }
        
            Projectile.velocity *= 0.1f;
        }
  
        if(Timer < 15)
        {
            Projectile.velocity -= Vector2.UnitY.RotatedBy(Timer * 0.15f  * Rand);
            Projectile.velocity.Y -= 0.05f;
            Projectile.velocity *= MathHelper.Lerp(1f, 0.8f, Timer / 15f);
            _startPosition = Projectile.velocity;
   
        } else if (EaseBackTimer < 60f)
        {
            if (this.OwnedByLocalClient())
            {
                _initialVelocity = (Main.MouseWorld - Projectile.Center);
                Projectile.netUpdate = true;
            }

            EaseBackTimer++;
            Vector2 targetVelocity = (_initialPosition + _initialVelocity * 60) - Projectile.Center;
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero) * 15;
            Vector2 targetPosition = Vector2.Lerp(_startPosition, targetVelocity, EasingFunction.InExpo(EaseBackTimer / 60f));
            Projectile.velocity = targetPosition;
            if(EaseBackTimer == 59)
            {

            }
        }
        else
        {
            Projectile.tileCollide = true;
            if (Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.4f;
            else
                Projectile.extraUpdates = 1;
        }
        if (Main.rand.NextBool(5))
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 
                        -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    break;
                case 1:
                    FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), 
                        -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                    sp2.gravity = 0f;
                    sp2.fast = true;
                    sp2.dampening = 0.1f;
                    break;
            }

        }
        Projectile.scale = MathHelper.SmoothStep(0f, 1f, Timer / 15f);
        if (Main.rand.NextBool(8))
        {
            FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.55f));
            sp.gravity = 0f;
            sp.fast = true;
            sp.dampening = 0.1f;
        }
        
        if (Main.rand.NextBool(8))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Scale: Main.rand.NextFloat(0.4f, 0.8f));
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Red * 0.5f;
        shader2.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Red * 0.5f;
        bloom.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.Red, Color.DarkRed, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f)) * 0.5f;
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(32, 2, completionRatio);
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    private void DrawPixelatedFlames(SpriteBatch sb, Vector2 screenPos)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        Vector2 strecthScale = new Vector2(1.5f, 0.7f);
        Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer glowBallDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        glowBallDrawer.color = glowColor;
        glowBallDrawer.scale *= 0.14f * strecthScale;
        glowBallDrawer.rotation = Projectile.rotation;
        Main.spriteBatch.Draw(glowBallDrawer);
        glowColor = Color.White;
        glowColor.A = 0;
        glowBallDrawer.color = glowColor;
        glowBallDrawer.scale *= 0.25f;
        Main.spriteBatch.Draw(glowBallDrawer);


        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        Color glowColor2 = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        spiralVortexDrawer.scale *= 0.14f;
        spiralVortexDrawer.color = glowColor;
        spiralVortexDrawer.rotation += Main.GlobalTimeWrappedHourly * 8;
        Main.spriteBatch.Draw(spiralVortexDrawer);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            Vector2 oldCenter = Projectile.oldPos[i] + Projectile.Size / 2f;
            Color afo = Color.Red;
            afo = Color.Lerp(afo, Color.Black, MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutExpo7(ratio)));
            afo.A = 0;
            afo *= 0.15f;
            glowBallDrawer.scale = Vector2.One * 0.1f * strecthScale;
            glowBallDrawer.worldPosition = oldCenter;
            glowBallDrawer.color = afo;
            Main.spriteBatch.Draw(glowBallDrawer);


            // spriteBatch.Draw(glowMask, oldCenter, null, afo, Projectile.oldRot[i], glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlames);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(int i = 0; i < 32; i++)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = -Projectile.oldVelocity;
            vel *= Main.rand.NextFloat(0.05f, 0.3f);
            Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: Main.rand.NextFloat(1f, 3f));
        }


        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.OrangeRed, Color.DarkRed, 45);
        fx.Scale *= Main.rand.NextFloat(0.4f, 0.6f);
        float numDust = 4;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
            dp.fast = true;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }

        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 vel = -(Projectile.oldPos[i] - Projectile.oldPos[i + 1]);
                vel = vel.RotatedByRandom(MathHelper.ToRadians(25));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(2, 7);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.DarkRed;
                spawnParams.scaleRange *= 0.4f;
                var dp = DustParticle.Spawn(Projectile.oldPos[i] + Projectile.Size * 0.5f, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;

            }
        }

        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity,
                ModContent.ProjectileType<EternalFlamePile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}

public class EternalFlamePile : ModProjectile
{
    private Vector2 _initialVelocity;
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle fireImpact = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireExplosion1");
            fireImpact = fireImpact with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(fireImpact, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 7);

            if (this.OwnedByLocalClient())
            {
                _initialVelocity = Projectile.velocity;
                Projectile.netUpdate = true;
            }
     
            Projectile.velocity = Vector2.Zero;
        }
        if (Main.rand.NextBool(16))
        {
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 32), -_initialVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 3f));
            fs.Center += _initialVelocity.SafeNormalize(Vector2.Zero) * 64;
            fs.fadeToColor = Color.Black * 0.35f;
            fs.color = Color.RosyBrown * 0.35f;
            fs.Scale *= 0.25f;
        }
            //  Vector2.c
            // Projectile.rotation = _initialVelocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(8))
        {
            DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32) - new Vector2(0, 12),
                -_initialVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
            sp.innerColor = Color.Yellow;
            sp.outerColor = Color.Red;
            sp.gravity = 0f;
            sp.fast = true;
            sp.dampening = 0.1f;
            sp.Scale *= 0.6f;
        }
        Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(4))
            target.AddBuff(BuffID.OnFire, 120);
    }
    private void DrawPixelatedFlames(SpriteBatch sb, Vector2 screenPos)
    {
       // var sb = Main.spriteBatch;
        float fade = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 30f));
        float inScale = EasingFunction.OutExpo(Timer / 30f);
        Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
        WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
        waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f + Projectile.whoAmI;
        waveShader.Amplitude = 0.3f;
        waveShader.Frequency = 8;
        waveShader.XStrength = 6;
        waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
        sb.Restart(effect: waveShader.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, Projectile.Center);
        drawer.rotation = Projectile.rotation;
        drawer.BottomCenterOrigin();
        drawer.color =Color.OrangeRed * fade * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI);
        drawer.color.A = 0;
        drawer.scale *= 0.5f * inScale;
        drawer.scale.Y *= ExtraMath.Osc(1f, 1.1f, offset: Projectile.whoAmI);
        if (Projectile.velocity.X < 0)
            drawer.spriteEffects = SpriteEffects.FlipHorizontally;
        sb.Draw(drawer);

        drawer.TopCenterOrigin();
        drawer.scale.Y *= 0.4f;
        drawer.spriteEffects |= SpriteEffects.FlipVertically;
        drawer.rotation = Projectile.rotation;
        sb.Draw(drawer);
        drawer.color = Color.Yellow * fade;
        drawer.color.A = 0;
        sb.Draw(drawer);


        sb.RestartDefaults();

        Asset<Texture2D> bloomLine = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bloomLine, Projectile.Center + new Vector2(0f, 12));
        //      drawer2.BottomCenterOrigin();
        drawer2.scale *= new Vector2(0.55f, 0.05f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * inScale;
        drawer2.color = Color.Yellow * fade; ;
        drawer2.color.A = 0;
        drawer2.rotation = Projectile.rotation;
        sb.Draw(drawer2);

        drawer2.scale = new Vector2(1.5f, 0.5f) * 0.35f;
        drawer2.color = Color.Red * fade; ;
        drawer2.color.A = 0;
        sb.Draw(drawer2);

        SpritebatchDrawer blastPillar = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.BlastPillar, Projectile.Center + new Vector2(0f, 12));
        blastPillar.BottomCenterOrigin();
        blastPillar.color = Color.Red * 0.5f * ExtraMath.Osc(0.6f, 1f, speed: 32, offset: Projectile.whoAmI) * fade;
        blastPillar.color.A = 0;
        blastPillar.scale *= 0.6f;
        blastPillar.rotation = Projectile.rotation;
        sb.Draw(blastPillar);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlames, DrawLayer.OverNPCsWithOutline);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
