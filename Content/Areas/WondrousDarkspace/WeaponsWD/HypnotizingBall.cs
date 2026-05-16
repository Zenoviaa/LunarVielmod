using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class HypnotizingAura : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle spawnSound = new SoundStyle("Stellamod/Assets/Sounds/Parendine2");
            spawnSound.PitchVariance = 0.3f;
            spawnSound.Volume = 0.5f;
            spawnSound.Pitch = -0.3f;
            SoundEngine.PlaySound(spawnSound, Projectile.position);
        }

        if (Timer % 12 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(32))
        {
            Vector2 initialVelocity = -Vector2.UnitY * 4;
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.Pink,
                outerColor = Color.Purple
            };

            Vector2 pos = Owner.position + new Vector2(Main.rand.Next(0, Owner.width), Main.rand.Next(0, Owner.height));
            DustParticle dp = DustParticle.Spawn(pos, initialVelocity, spawnParams);
            dp.gravity = 0f;
            dp.dampening = 0.05f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(6))
        {
            target.AddBuff(BuffID.ShadowFlame, 30);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        return false;
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircle2;
        Vector2 drawOrigin = noise.Size() / 2f;
        Texture2D texture = noise.Value;

        Vector2 drawCenter = Projectile.Center - Main.screenPosition;
        drawCenter.Y += Owner.gfxOffY;

        float ease = EasingFunction.InOutSine((float)Projectile.timeLeft / 60f) * EasingFunction.InOutSine(Timer / 10f);
        Color drawColor = Color.White;
        drawColor.A = 0;
        Color drawColor2 = Color.Blue;
        drawColor2.A = 0;
        //     drawColor *= 0.5f;

        Vector2 scale = Vector2.One;
        scale *= ease;
        scale *= 4;
        var shader = CelestialAuraShader.Instance;
        shader.InnerColor = Color.Purple;
        shader.OuterColor = Color.Black;
        shader.Time = -Main.GlobalTimeWrappedHourly;
        shader.Tiling = Vector2.One * 0.1f;
        spriteBatch.Restart(effect: shader.Effect);
        for (float f = 0; f < 3; f++)
        {
            Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
            glowColor.A = 0;
            float rotOffset = (f / 4f) * MathHelper.TwoPi;
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}


public class HypnotizingBall : BaseChainedBallItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 17;
        Item.shoot = ModContent.ProjectileType<HypnotizingBallProj>();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            HypnotizedSoul,
            BlankOrb>();
    }
}

public class HypnotizingBallProj : BaseChainedBallProjectile
{
    private bool _hit;
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Just having this here in case
        //Iron Ball is just gonna use default stuff htough

        //Variables
        //Easing
        easer = (float lerpValue) => Easing.InOutExpo(lerpValue, 7);

        //How far it drags behind you
        dragDistance = 126;

        //Swing Range (IT USES OVAL SWING)
        swingRange = MathHelper.ToRadians(360);

        //Offst for theoval swing
        ovalRotOffset = MathHelper.ToRadians(-90);

        //Max X Swing Radius
        swingXRadius = 600;

        //Y Swing  Radius
        swingYRadius = 80;

        //How long it takes to swing
        baseSwingTime = 48;

        //Glowing stuff
        glowDistanceOffset = 4;
        glowRotationSpeed = 0.005f;

        //Damage multiplier for hitting the tip
        TipDamageMultiplier = 2;
    }


    protected override void SetSlingDefaults()
    {
        base.SetSlingDefaults();

        //Reset the hit
        _hit = false;
    }


    public override void AI()
    {
        base.AI();

        if (Main.rand.NextBool(8))
        {
            switch (Main.rand.Next(2))
            {
                case 0:
                    DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                    sp.outerColor = Color.Pink;
                    sp.gravity = 0f;
                    sp.fast = true;
                    sp.dampening = 0.1f;
                    sp.Scale *= 0.33f;
                    break;
                case 1:
                    FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                    sp2.innerColor = Color.Pink;
                    sp2.outerColor = Color.DarkViolet;
                    sp2.gravity = 0f;
                    sp2.fast = true;
                    sp2.dampening = 0.1f;
                    sp2.Scale *= 0.13f;
                    break;
            }
        }
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0, 32, completionRatio) * EasingFunction.QuadraticBump(unEasedLerpValue);
    }

    private Color GetTrailColor(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
        return trailColor;
    }

    protected override void DrawSlashTrail(ref Color lightColor, Vector2[] slashPos)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.Lerp(Color.Pink, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 8));
        laserShader.InnerColor = Color.DarkBlue;
        laserShader.OuterColor = Color.DarkViolet;
        TrailDrawer.Draw(Main.spriteBatch, slashPos, GetTrailColor, GetTrailWidth, laserShader);
    }

    protected override void DrawBallSprite(ref Color lightColor)
    {
        base.DrawBallSprite(ref lightColor);
        Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 drawOrigin = glowTexture.Size() * 0.5f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Vector2 drawCenter = Projectile.Center - Main.screenPosition;
        Color glowColor = Color.Pink;
        glowColor.A = 0;
        glowColor *= 0.5f;
        spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.15f, SpriteEffects.None, 0);
    }

    public override void OnTipper(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.OnTipper(target, ref modifiers);
        float damage = Projectile.damage * 0.3f;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
            ModContent.ProjectileType<HypnotizingAura>(), (int)damage, Projectile.knockBack, Projectile.owner);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);


        if (!_hit)
        {
            float numDust = 6;
            for (float n = 0; n < numDust; n++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Pink,
                    outerColor = Color.DarkViolet
                };
                DustParticle.Spawn(target.Center, -Vector2.UnitY.RotatedByRandom(1.5f) * Main.rand.NextFloat(2f, 8f), spawnParams);
            }
            SoundStyle hitSound;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    hitSound = AssetManager.GetSound("Fire/FireballShoot1");
                    break;
                case 1:
                    hitSound = AssetManager.GetSound("Fire/FireballShoot2");
                    break;
            }

            hitSound.Pitch = 0.5f;
            hitSound.PitchVariance = 0.3f;
            hitSound.Volume = 0.66f;
            SoundEngine.PlaySound(hitSound, target.Center);
            FXUtil.ShakeCamera(target.Center, 1024, 2);
            _hit = true;
        }
        if (Main.rand.NextBool(2))
            target.AddBuff(BuffID.Confused, 120);
    }
}