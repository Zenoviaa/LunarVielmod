using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;


internal class BlastingBlossom : ModNPC
{
    private enum AIState
    {
        Idle,
        Cover,
        EyePeek,
        BlastUncover
    }


    private float _lanternScale;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get
        {
            return (AIState)NPC.ai[1];
        }
        set
        {
            NPC.ai[1] = (float)value;
        }
    }
    private Outliner _outliner;
    private Vector2 _facingDirection;
    private Player MyTarget => Main.player[NPC.target];
    private int BlastingBlossomDamage => 24;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        this.AddToAbyss();
        NPCSets.Heavy[Type] = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 36;
        NPC.height = 36;
        NPC.damage = 34;
        NPC.defense = 8;
        NPC.lifeMax = 140;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 563f;
        NPC.knockBackResist = 0f;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        NPC.TargetClosest(faceTarget: false);

        this.SetDrawOrigin(new Vector2(27, 36));
        NPC.spriteDirection = NPC.direction;
        var anchor = FindClosestTile();
        _facingDirection = anchor.facingDirection;
        Vector2 posWorld = anchor.anchorTile.ToWorldCoordinates();
        NPC.rotation = anchor.facingDirection.ToRotation() + MathHelper.PiOver2;
        NPC.Center = posWorld + anchor.facingDirection * 16;
        NPC.velocity = Vector2.Zero;
        _outliner.SetDefaults();

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();

                break;
            case AIState.Cover:
                AI_Cover();
                break;
            case AIState.EyePeek:
                AI_EyePeek();
                break;
            case AIState.BlastUncover:
                AI_BlastUncover();
                break;
        }

        _outliner.Update();

    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private (Point anchorTile, Vector2 facingDirection) FindClosestTile()
    {
        bool IsHit(in Point p)
        {
            return WorldGen.InWorld(p.X, p.Y) && Main.tile[p].HasTile && Main.tileSolid[Main.tile[p].TileType];
        }

        Point center = NPC.Center.ToTileCoordinates();
        for (int d = 0; d < 50; d++)
        {

            Point up = new Point(0, -d);
            Point down = new Point(0, d);
            Point left = new Point(-d, 0);
            Point right = new Point(d, 0);

            up += center;
            down += center;
            left += center;
            right += center;


            if (IsHit(up))
                return (up, Vector2.UnitY);
            if (IsHit(down))
                return (down, -Vector2.UnitY);
            if (IsHit(left))
                return (left, Vector2.UnitX);
            if (IsHit(right))
                return (right, -Vector2.UnitX);
        }
        return (NPC.Center.ToTileCoordinates(), -Vector2.UnitY);
    }
    private void AI_Idle()
    {
        Timer++;
        Vector2 dirToTarget = (MyTarget.Center - NPC.Center);
        dirToTarget = dirToTarget.SafeNormalize(Vector2.Zero);
        Vector2 facingDirection = _facingDirection;
        float dp = Vector2.Dot(dirToTarget, facingDirection);
        if (dp > 0.5f)
        {
            SwitchState(AIState.Cover);
        }
        this.GetAnimator().PlayAnimation("Idle");
    }

    private void AI_Cover()
    {
        Timer++;
        this.GetAnimator().PlayAnimation("Cover", AnimationParams.Default with { IsLooping = false });
        if (Timer >= 60)
        {
            SwitchState(AIState.EyePeek);
        }
    }

    private void AI_EyePeek()
    {
        Timer++;
        _outliner.warning = true;
        this.GetAnimator().PlayAnimation("EyePeek", AnimationParams.Default with { IsLooping = false });
        if (Timer >= 60)
        {
            SwitchState(AIState.BlastUncover);
        }
    }

    private void AI_BlastUncover()
    {
        Timer++;
        _outliner.attacking = true;
        if (Timer == 12 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + _facingDirection * 16, _facingDirection,
                ModContent.ProjectileType<BlastingBlossomBeam>(), BlastingBlossomDamage, 1, Main.myPlayer, ai1: 30);
        }
        this.GetAnimator().PlayAnimation("BlastUncover", AnimationParams.Default with { IsLooping = false });
        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    private void DrawWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center + Vector2.UnitX * NPC.direction * 18 * _lanternScale);
        drawer.color = Color.PaleTurquoise * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.2f * _lanternScale;
        drawer.color.A = 0;
        drawer.scale *= 0.5f * _lanternScale;
        spriteBatch.Draw(drawer);
        OutlineRenderer.Queue(DrawWhite);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}



public class BlastingBlossomBeam : ModProjectile
{
    private float BeamLength;
    private Vector2[] _beamPoints = null!;
    private Vector2[] BeamPoints
    {
        get
        {
            _beamPoints ??= new Vector2[32];

            for (int i = 0; i < _beamPoints.Length; i++)
            {
                float ratio = i / (float)_beamPoints.Length;
                _beamPoints[i] = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength, ratio);
            }
            return _beamPoints;
        }
    }
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];

    private float Lifetime => Projectile.ai[1];
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return ProjectileHelper.OldPosColliding(BeamPoints, projHitbox, targetHitbox);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.scale = EasingFunction.QuadraticBump(Timer / Lifetime);
        ShakeScreenPosition.Shake = MathHelper.SmoothStep(5, 0, Timer / Lifetime);
        float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, 2000);
        BeamLength = targetBeamLength;

        if (Main.rand.NextBool(8))
        {
            int index = Main.rand.Next(0, BeamPoints.Length);
            Vector2 b = BeamPoints[index];
            var sp = SparkleParticle.Spawn(b, Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f));
            sp.Scale *= 0.85f;
            sp.dampening = 0.1f;
            sp.flickering = true;
            sp.innerColor = Color.White;
            sp.outerColor = Color.Blue;
            sp.gravity = 0f;
            sp.fast = true;
        }



        if (Timer == 19)
        {
            for (int i = 0; i < BeamPoints.Length; i++)
            {
                Vector2 beamPoint = BeamPoints[i];
                if (Main.rand.NextBool(3))
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.LightCyan,
                        outerColor = Color.DarkBlue,
                        gravity = 0f,
                        scaleRange = new Vector2(1f, 3f)
                    };
                    var dp = DustParticle.Spawn(beamPoint, Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.5f), spawnParams);
                    dp.dampening = 0.1f;
                }
                if (Main.rand.NextBool(4))
                {
                    var dp = SparkleParticle.Spawn(beamPoint, Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.25f, 0.5f));
                    dp.Scale *= 0.85f;
                    dp.dampening = 0.1f;
                    dp.flickering = true;
                    dp.innerColor = Color.White;
                    dp.outerColor = Color.Blue;
                    dp.gravity = 0f;
                }
            }
        }
        if (Timer == 1)
        {
            for (int i = 0; i < BeamPoints.Length; i++)
            {
                Vector2 beamPoint = BeamPoints[i];
                if (Main.rand.NextBool(6))
                {
                    GlowDonutParticle glowDonutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(beamPoint, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(1.5f) * Main.rand.NextFloat(0.2f, 0.6f), Scale: 0.4f);
                    glowDonutParticle.rotOffset = MathHelper.PiOver2;

                    float ratio = i / (float)BeamPoints.Length;
                    glowDonutParticle.Scale *= MathHelper.Lerp(1f, 0.25f, ratio);

                }
            }
            SoundStyle sound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
            sound.PitchVariance = 0.3f;
            sound.Volume = 0.5f;
            SoundEngine.PlaySound(sound, Projectile.position);

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
            for (float f = 0; f < 16; f++)
            {
                Vector2 initialVelocity = -Projectile.velocity * 4;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                DustParticle dustParticle = Particle<DustParticle>.Spawn(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }

            for (float f = 0; f < 6; f++)
            {
                Vector2 initialVelocity = -Vector2.UnitY;
                initialVelocity *= 12;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(360));
                initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                SparkParticle dustParticle = LegacyParticle.NewParticle<SparkParticle>(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 2f));
                dustParticle.innerColor = Color.SkyBlue;
                dustParticle.outerColor = Color.Violet;
            }

            for (float f = 0; f < 6; f++)
            {
                Vector2 initialVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                initialVelocity *= 4;
                initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(explosionCenter + initialVelocity,
                    initialVelocity, Color.White, Scale: Main.rand.NextFloat(0.6f, 1.3f));
                smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                smokeParticle.fadeToColor = Color.Black;
            }

            FXUtil.GlowCircleBoom(explosionCenter,
                innerColor: Color.White,
                glowColor: Color.LightSkyBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: 0.06f);

            for (float f = 0; f < 3f; f++)
            {
                float progress = f / 3f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;

            }
        }
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Aqua, completionRatio);
    }

    private float GetTrailWidth(float completionRatio)
    {
        return 32 * EasingFunction.QuadraticBump(Timer / Lifetime);
    }

    private void DrawPixelGlows(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D glow = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 drawOrigin = glow.Size() * 0.5f;
        for (int i = 0; i < BeamPoints.Length; i++)
        {
            if (i % 2 == 0)
            {
                Vector2 pos = BeamPoints[i];
                Color color = Color.DarkBlue;
                color.A = 0;
                spriteBatch.Draw(glow, pos - screenPos, null, color, 0, drawOrigin, 0.25f * new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.None, 0);
            }

        }
        Color muzzleColor = Color.DarkBlue;
        muzzleColor.A = 0;
        spriteBatch.Draw(glow, Projectile.Center - screenPos, null, muzzleColor, 0, drawOrigin, 0.15f * new Vector2(1f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);

        muzzleColor = Color.White;
        muzzleColor.A = 0;
        spriteBatch.Draw(glow, Projectile.Center - screenPos, null, muzzleColor, 0, drawOrigin, 0.1f * new Vector2(1f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);


        muzzleColor = Color.DarkBlue;
        muzzleColor.A = 0;
        Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
        spriteBatch.Draw(glow, Projectile.Center + offset - screenPos, null, muzzleColor, 0, drawOrigin, 0.15f * new Vector2(1.75f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);

        muzzleColor = Color.White;
        muzzleColor.A = 0;
        spriteBatch.Draw(glow, Projectile.Center + offset - screenPos, null, muzzleColor, 0, drawOrigin, 0.1f * new Vector2(1.75f, 1.75f) * Projectile.scale, SpriteEffects.None, 0);
    }


    private void DrawPixelatedBeam(GraphicsDevice graphicsDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.LightBlue;
        shader2.InnerColor = Color.Blue;
        shader2.OuterColor = Color.DarkBlue;
        shader2.LaserTexture = TrailRegistry.SpikyTrail1;
        TrailDrawer.Draw(Main.spriteBatch, BeamPoints, GetTrailColor, GetTrailWidth, shader2);
    
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelGlows);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
