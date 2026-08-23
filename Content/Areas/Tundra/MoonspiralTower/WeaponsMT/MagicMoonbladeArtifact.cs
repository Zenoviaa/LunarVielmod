using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Buffers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class MagicMoonbladeArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.width = 16;
        Item.height = 16;
        Item.mana = 25;
        Item.damage = 38;
        Item.useAnimation = Item.useTime = 15;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.crit = 4;
        Item.shoot = ModContent.ProjectileType<MagicMoonblade>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }


    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        Vector2 originalVelocity = velocity;
        velocity = Main.MouseWorld;

    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankStaff>();
    }
}


public class MagicMoonblade : ModProjectile
{
    private enum SwingState
    {
        ThrowOut,
        Swing
    }
    private int _direction;
    private float _scale;
    private float _alpha;
    private float _initialRotation;
    private Vector2 _initialPosition;
    private Vector2 _swingCenter;
    private Vector2 _randOffset;
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private float Mult => 1;
    private SwingState State
    {
        get => (SwingState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialPosition = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = (int)(Mult - 1);
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case SwingState.ThrowOut:
                AI_ThrowOut();
                break;
            case SwingState.Swing:
                AI_Swing();
                break;
        }
    }

    private void SwitchState(SwingState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }
    private void AI_ThrowOut()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            softSummon.PitchVariance = 0.3f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
            _swingTrailCache = ArrayPool<Vector2>.Shared.Rent(200);
            for (int i = 0; i < _swingTrailCache.Length; i++)
            {
                _swingTrailCache[i] = Vector2.Zero;
            }
            _initialPosition = Projectile.Center;
            _randOffset = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 75;
        }
        if (Timer % 8 == 0)
        {
            float range = Main.rand.NextFloat(48, 64);
            Vector2 pos2 = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
            var sp = SparkleParticle.Spawn(pos2, Vector2.Zero);
            sp.outerColor = Color.LightSkyBlue;
            sp.gravity = 0;
            sp.Scale *= 0.2f;
            sp.fast = true;
        }

        Vector2 targetSwingStart = Projectile.velocity + _randOffset;
        float easeinTime = Vector2.Distance(_initialPosition, targetSwingStart) / 8f;
        easeinTime *= Mult;
        float ease = EasingFunction.InOutExpo(Timer / easeinTime);
        Vector2 pos = Vector2.Lerp(_initialPosition, targetSwingStart, ease);
        _direction = (targetSwingStart.X > _initialPosition.X) ? 1 : -1;
        _scale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 30f));
        _alpha = 1f;
        Projectile.friendly = false;
        Projectile.Center = pos;
        Projectile.rotation += MathHelper.Lerp(0.15f, 0.25f, EasingFunction.InOutSine(Timer / easeinTime)) / Mult;
        if (Timer >= easeinTime)
        {
            SwitchState(SwingState.Swing);
        }
    }
    private Vector2[] _swingTrailCache;
    private OvalSwing _ovalSwing;
    private void AI_Swing()
    {
        if (_swingTrailCache == null)
            return;

        Timer++;
        if (Timer == 1)
        {
            _initialPosition = Projectile.velocity + _randOffset; ;
            _initialRotation = Projectile.rotation;
            _swingCenter = Projectile.velocity;

            SoundStyle impactSound;
            switch (Main.rand.Next(5))
            {
                default:
                case 0:
                    impactSound = AssetRegistry.Sounds.Melee.SwordSwing1;
                    break;
                case 1:
                    impactSound = AssetRegistry.Sounds.Melee.SwordSwing2;
                    break;
                case 2:
                    impactSound = AssetRegistry.Sounds.Melee.SwordSwing3;
                    break;
                case 3:
                    impactSound = AssetRegistry.Sounds.Melee.SwordSwing4;
                    break;
                case 4:
                    impactSound = AssetRegistry.Sounds.Melee.SwordSwing5;
                    break;
            }
            impactSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(impactSound, Projectile.position);
        }

        Vector2 forwardVelocity = Projectile.velocity - _initialPosition;
        forwardVelocity = forwardVelocity.SafeNormalize(Vector2.Zero);
        Vector2 upVelocity = forwardVelocity.RotatedBy(MathHelper.ToRadians(-90));
        upVelocity *= 80;

        Vector2 startSwing = _swingCenter + upVelocity;
        Vector2 endSwing = _swingCenter - upVelocity * 2;
        float swingTime = 60f * Mult;

        _ovalSwing ??= new OvalSwing();
        _ovalSwing.XSwingRadius = 64;
        _ovalSwing.YSwingRadius = 128;
        _ovalSwing.SwingDegrees = 270;
        _ovalSwing.Duration = 45;
        _ovalSwing.SetDirection(_direction);

        float interpolant = Timer / swingTime;
        _ovalSwing.UpdateSwing(interpolant, _initialPosition, forwardVelocity, out Vector2 o);

        _ovalSwing.CalculateTrailingPointsExtended(interpolant, forwardVelocity, ref _swingTrailCache,
            trailOffset: 1f);
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(_initialPosition.X, _initialPosition.Y, 0));

        for (int t = 0; t < _swingTrailCache.Length; t++)
        {
            ref Vector2 point = ref _swingTrailCache[t];
            point = Vector2.Transform(point, translationMatrix);
        }

        Vector2 interp = Vector2.Lerp(_initialPosition, _initialPosition + o, EasingFunction.OutSine(Timer / 20f * Mult));
        _alpha = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 60f * Mult));
        Projectile.friendly = true;
        Projectile.Center = interp;
        Projectile.rotation = (Projectile.Center - _initialPosition).ToRotation() + MathHelper.PiOver4;

        if (Timer >= swingTime)
        {
            Projectile.Kill();
        }
    }

    private Color GetTrailColor(float completionRatio)
    {
        Color color = Color.Lerp(Color.White, Color.LightSkyBlue, completionRatio);
        float alpha = EasingFunction.QuadraticBump(completionRatio);
        return color * alpha;
    }

    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 0, completionRatio) * Projectile.scale;
    }

    private void DrawPixelatedTrail(GraphicsDevice graphicsDevice)
    {
        var shader = BasicLaserShader.Instance;
        shader.LaserTexture = TrailRegistry.StarTrail;
        shader.InnerColor = Color.White;
        shader.OuterColor = Color.SkyBlue;
        // TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size / 2f);
    }
    private Color GetSlashTrailColor(float w)
    {
        Color slashColor = Color.Lerp(Color.White, Color.Black, w);
        //       slashColor = Color.Lerp(Color.Black, Color.White, flashRatio);
        return slashColor;
    }

    private float GetSlashTrailWidth(float w)
    {
        return 48 * MathHelper.Lerp(0, 1, EasingFunction.InOutSine(w)) * EasingFunction.QuadraticBump(Timer / 60f * Mult);
    }
    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.Blue, 0.5f), Color.Blue, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.DeepSkyBlue, ratio) * EasingFunction.QuadraticBump(ratio);
    }
    private float GetBloomWidth(float w)
    {
        return 100 * MathHelper.Lerp(0, 1, EasingFunction.InOutSine(w)) * EasingFunction.QuadraticBump(Timer / 60f * Mult);
    }

    private float SlashEffectWidth(float ratio)
    {
        return EasingFunction.QuadraticBump(ratio) * 16 * EasingFunction.OutExpo(Timer / 30f * Mult);
    }

    private Color SlashEffectColor(float ratio)
    {
        Color lerp1 = Color.Lerp(Color.LightCyan, Color.Blue, ratio);
        return Color.Lerp(Color.Transparent, lerp1, ratio) * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 60f * Mult)) * _alpha;
    }
    private SlashTrailer _slashTrailer;
    private void DrawSlashTrail(GraphicsDevice gDevice)
    {
        Color lightColor = Color.White;
        if (_swingTrailCache == null)
            return;


        //   _auraTrailer.DrawTrail(ref lightColor, _swingTrailCache);
        //    _wideTrailer.DrawTrail(ref lightColor, _swingTrailCache);
        float flashRatio = EasingFunction.QuadraticBump(Timer / 60f * Mult);
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.OuterColor = Color.Lerp(Color.Black, Color.LightBlue, flashRatio);
        laserShader.InnerColor = Color.Lerp(Color.Black, Color.White, flashRatio);
        //laserShader.LaserColor = Color.Lerp(Color.Black, Color.Lerp(Color.White, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 8) * 0.5f), flashRatio);
        laserShader.LaserTexture = TrailRegistry.BeamTrail;
        //laserShader.BloomTexture = TrailRegistry.BeamTrail;
        laserShader.Time = Main.GlobalTimeWrappedHourly * -64;
        TrailDrawer.Draw(Main.spriteBatch, _swingTrailCache, GetSlashTrailColor, GetSlashTrailWidth, laserShader);

        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.Blue;
        bloomTrailShader.OuterColor = Color.Blue;
        TrailDrawer.Draw(Main.spriteBatch, _swingTrailCache, GetBloomColor, GetBloomWidth, bloomTrailShader);
        _slashTrailer ??= new SlashTrailer
        {
            Shader = new SlashEffect()
            {
                BaseColor = Color.LightGreen,
                HighlightColor = Color.White,
                RimHighlightColor = Color.LightBlue,
                WindColor = Color.DarkBlue,
                BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive,
                WindTexture = TrailRegistry.CrystalTrail.Value
            },
            TrailWidthFunction = SlashEffectWidth,
            TrailColorFunction = SlashEffectColor
        };

        Color color = Color.White;
        _slashTrailer.DrawTrail(ref color, _swingTrailCache);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSlashTrail, DrawLayer.OverNPCsAdditive);

        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail, DrawLayer.OverNPCsWithOutline);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color *= _alpha;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 0.4f * _scale;
        Main.spriteBatch.Draw(backGlowDrawwer);


        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        spiralDrawer.color = Color.SkyBlue * 0.15f;
        spiralDrawer.color *= _alpha;
        spiralDrawer.color.A = 0;
        spiralDrawer.scale = Vector2.One * 0.4f * _scale;
        spiralDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(spiralDrawer);

        SpritebatchDrawer aura2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Owner.Center);
        aura2.color = Color.DarkBlue * 0.15f;
        aura2.color *= _alpha;
        aura2.color.A = 0;
        aura2.scale = Vector2.One * 0.4f * _scale;
        aura2.rotation += Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(aura2);


        SpritebatchDrawer magicCircleDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MagicSwordCircle, Owner.Center);
        magicCircleDrawer.color = Color.SkyBlue * 0.15f;
        magicCircleDrawer.color *= _alpha;
        magicCircleDrawer.color.A = 0;
        magicCircleDrawer.scale = Vector2.One * 0.4f * _scale;
        magicCircleDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(magicCircleDrawer);


        Asset<Texture2D> textureAsset = TextureAssets.Item[ModContent.ItemType<MagicMoonbladeArtifact>()];

        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(textureAsset, Projectile.Center);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            drawer2.worldPosition = pos + Projectile.Size * 0.5f;
            drawer2.rotation = Projectile.oldRot[i];
            float ratio = i / (float)Projectile.oldPos.Length;
            ratio = 1f - ratio;
            drawer2.color = Color.White * ratio * 0.1f;
            drawer2.color *= _alpha;
            drawer2.color.A = 0;
            Main.spriteBatch.Draw(drawer2);
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, Projectile.Center);

        drawer.rotation = Projectile.rotation;
        drawer.scale = _scale * Vector2.One;
        drawer.color *= ExtraMath.Osc(0.2f, 0.8f, speed: 16, Projectile.whoAmI * 4);
        drawer.color *= _alpha;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundStyle impactSound;
        switch (Main.rand.Next(4))
        {
            default:
            case 0:
                impactSound = AssetRegistry.Sounds.Melee.SwordHit1;
                break;
            case 1:
                impactSound = AssetRegistry.Sounds.Melee.SwordHit2;
                break;
            case 2:
                impactSound = AssetRegistry.Sounds.Melee.SwordHit3;
                break;
            case 3:
                impactSound = AssetRegistry.Sounds.Melee.SwordHit4;
                break;
        }
        impactSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(impactSound, target.position);
        float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
        for (float n = 0; n < 3; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.LightSkyBlue;
            spawnParams.outerColor = Color.DarkBlue;
            spawnParams.scaleRange = new Vector2(0.3f, 1f);
            DustParticle.Spawn(target.Center, Main.rand.NextVector2Circular(4, 4) * Main.rand.NextFloat(0.5f, 1f) * 0.3f, spawnParams);
        }

        var fx = FXUtil.GlowStretch(target.Center, _randOffset.RotatedBy(MathHelper.ToRadians(90)));
        fx.Velocity *= 0.2f;
        fx.VectorScale.X *= 3;

        SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(target.Center, -Vector2.UnitY, Color.White, Scale: 1f);
        sp.initialColor = Color.White * 0.14f;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (_swingTrailCache == null)
            return;
        ArrayPool<Vector2>.Shared.Return(_swingTrailCache);
    }
}