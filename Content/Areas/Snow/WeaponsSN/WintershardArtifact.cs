using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Buffers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.WeaponsSN;

[Autoload(Side = ModSide.Client)]
public class LineStreakRenderer : ModSystem
{
    private float _spawnTimer;
    private LineStreak _streak;
    public class LineStreak
    {
        public static int TrailCacheLength => 16;
        public LineStreak(int max)
        {
            length = max;
            target = new Entity[max];
            position = new Vector2[max];
            oldPos = new Vector2[max, TrailCacheLength];
            timer = new float[max];
            active = new bool[max];
        }

        public int length;
        public Entity[] target;
        public Vector2[] position;
        public Vector2[,] oldPos;
        public float[] timer;
        public bool[] active;
        public void Resize(int newLength)
        {
            length = newLength;
            target = new Entity[length];
            position = new Vector2[length];
            oldPos = new Vector2[length, TrailCacheLength];
            timer = new float[length];
            active = new bool[length];
        }
    }

    public override void Load()
    {
        base.Load();
        if (Main.dedServ)
            return;

        _streak = new LineStreak(50);
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();

        //This looks a bit strange
        //But basically what we're doing is skipping the entire calculation if none of these have been spawned recently
        //Don't run code that doesn't need to be ran after all
        //Should probably implement this for our other vfx systems
        if (_spawnTimer <= 0)
            return;
        _spawnTimer--;
        int length = LineStreak.TrailCacheLength;
        float maxSpeed = 25;
        for (int i = 0; i < _streak.length; i++)
        {
            ref bool isActive = ref _streak.active[i];
            if (!isActive)
                continue;


            //Check if this streak needs to be killed
            ref Vector2 position = ref _streak.position[i];
            ref float timer = ref _streak.timer[i];

            Entity entity = _streak.target[i];
            if(entity == null)
            {
                //Progress old pos ararys
                for (int j = length - 1; j > 0; j--)
                {
                    _streak.oldPos[i, j] = _streak.oldPos[i, j - 1];
                }

                _streak.oldPos[i, 0] = position;
                timer++;
                if(timer >= length)
                {
                    isActive = false;
                }
                continue;
            }

            Vector2 target = entity.Center;
            float distanceToTarget = Vector2.Distance(position, target);
            if (distanceToTarget <= 16)
            {
                _streak.target[i] = null;
                timer = 0;
                continue;
            }
  
            timer++; 
            Vector2 direction = (target - position).SafeNormalize(Vector2.Zero);
            float easeIn = EasingFunction.InExpo(timer / 45f);
            float speed = MathHelper.Lerp(0f, maxSpeed, easeIn);
   
            if (speed > distanceToTarget)
                speed = distanceToTarget;
            Vector2 velocity = direction * speed;

            //Progress old pos ararys
            for (int j = length - 1; j > 0; j--)
            {
                _streak.oldPos[i, j] = _streak.oldPos[i, j - 1];
            }
                
            _streak.oldPos[i, 0] = position;
            position += velocity;
        }
        PixelationManager.QueuePrimitivesDrawAction(RenderStreaks, DrawLayer.OverPlayers);
    }

    public void NewStreak(Vector2 position, float time, Entity target)
    {
        int index = 0;
        for(int i = 0; i < _streak.length; i++)
        {
            ref bool isActive = ref _streak.active[i];
            if (isActive)
                continue;
            index = i;
            isActive = true;
            break;
        }

        _streak.position[index] = position;
        _streak.target[index] = target;
        for(int j = 0; j < LineStreak.TrailCacheLength; j++)
        {
            _streak.oldPos[index, j] = position;
        }
        if (_spawnTimer < time)
            _spawnTimer = time;
    }

    public Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio));
    }

    public float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0f, 16, EasingFunction.QuadraticBump(completionRatio));
    }
    
    public void RenderStreaks(GraphicsDevice graphicsDevice)
    {
        SimpleTrailShader tShader = SimpleTrailShader.Instance;
        Vector2[] oldPos = ArrayPool<Vector2>.Shared.Rent(LineStreak.TrailCacheLength);

        //Could batch these tbf
        for(int i = 0; i < _streak.length; i++)
        {
            ref bool isActive = ref _streak.active[i];
            if (!isActive)
                continue;
            for(int j = 0; j < LineStreak.TrailCacheLength; j++)
            {
                oldPos[j] = _streak.oldPos[i, j];
            }
            TrailDrawer.Draw(Main.spriteBatch, oldPos, GetTrailColor, GetTrailWidth, tShader);
        }


        ArrayPool<Vector2>.Shared.Return(oldPos);
    }
}
public class WintershardArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.width = 16;
        Item.height = 16;
        Item.channel = true;
        Item.autoReuse = false;
        Item.mana = 52;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.crit = 4;
        Item.shoot = ModContent.ProjectileType<WintershardClump>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool AltFunctionUse(Player player)
    {
   
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if(player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<WintershardWave>();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class WintershardWave : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
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
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.Center = Owner.Center;
        Projectile.velocity = Vector2.Zero;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio));
        waveDrawer.color = Color.SkyBlue;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 1f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
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
public class MagicCircleRenderer
{
    private TexturedQuad _texturedQuad;
    public TexturedQuad TexturedQuad
    {
        get
        {
            _texturedQuad ??= new TexturedQuad();
            return _texturedQuad;
        }
    }
    public MagicCircleRenderer(Asset<Texture2D> ringTexture)
    {
        ringTextureAsset = ringTexture;
    }

    public Asset<Texture2D> ringTextureAsset;
    public void DrawRing(Vector2 center, Vector2 velocity, int frame, float numFrames, Color color, float perpsectiveRotation)
    {
        MagicCircleShader magicCircleShader = MagicCircleShader.Instance;
        //Here we need to prepare the shader
        float f = frame;
        Vector2 tiling = new Vector2(1f, 1f / numFrames);
        Vector2 offset = new Vector2(0, f * 1f / numFrames);
        Vector4 tilingOffset = new Vector4(offset.X, offset.Y, tiling.X, tiling.Y);
        magicCircleShader.TilingOffset = tilingOffset;
        magicCircleShader.RingTexture = ringTextureAsset;
        TexturedQuad.CalculatePerspectiveCenterVertices(center, 120, 120, velocity.ToRotation(), perpsectiveRotation);
        TexturedQuad.SetColor(color);
        TexturedQuad.DrawWithShader(magicCircleShader);
    }
}
public class WintershardClump : ModProjectile
{
    private enum ChargeState
    {
        No_Charge,
        Level_1,
        Level_2,
        Level_3
    }

    private MagicCircleRenderer _magicCircleRenderer;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ChargeState State
    {
        get => (ChargeState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private ref float GlobalTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.scale = 0.001f;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        GlobalTimer++;
        switch (State)
        {
            case ChargeState.No_Charge:
                AI_NoCharge();
                break;
            case ChargeState.Level_1:
                AI_ChargeLevel1();
                break;
            case ChargeState.Level_2:
                AI_ChargeLevel2();
                break;
            case ChargeState.Level_3:
                AI_ChargeLevel3();
                break;
        }
        AI_HoldAnimation();
        AI_TryBreak();
    }

    private void AI_NoCharge()
    {
        Timer++;
        if (Timer % 6 == 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float range = Main.rand.NextFloat(80, 128);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                LineStreakRenderer lineStreakRenderer = ModContent.GetInstance<LineStreakRenderer>();
                lineStreakRenderer.NewStreak(pos, 60, Projectile);
            }
        }
        if (Timer == 1)
        {
            SoundStyle chargeSound = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
            chargeSound.Volume = 0.5f;
            chargeSound.Pitch = -0.75f;
            SoundEngine.PlaySound(chargeSound, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            float range = Main.rand.NextFloat(80, 128);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams();
            spawnParams.outerColor = Color.LightSkyBlue;
            spawnParams.scaleRange *= 0.3f;
            spawnParams.gravity = 0;
            DustParticle.Spawn(pos, Vector2.Zero, spawnParams);
        }
        Projectile.scale = MathHelper.Lerp(Projectile.scale, 0f, 0.1f);
        if (Timer >= 60)
        {
            SwitchState(ChargeState.Level_1);
        }
    }

    private void EnclosingCircle()
    {
        /*
        if(Main.netMode != NetmodeID.Server)
            ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.LightSkyBlue, 0.02f, 15);*/
        SoundStyle growSound;
        int index = (int)State;
        index -= 1;
        switch (index)
        {
            default:
            case 0:
                growSound = AssetRegistry.Sounds.Illuria.SlushShot1;
                break;
            case 1:
                growSound = AssetRegistry.Sounds.Illuria.SlushShot2;
                break;
            case 2:
                growSound = AssetRegistry.Sounds.Illuria.SlushShot3;
                break;
        }
        SoundEngine.PlaySound(growSound, Projectile.position);
        var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
        part.Scale *= 1;
        part.shrink = true;
        part.noStretch = true;
        part.fadeToColor = Color.DarkBlue * 0.3f;
        part.innerColor = Color.White * 0.3f;
        part.outerColor = Color.SkyBlue * 0.3f;
    }
    private void AI_ChargeLevel1()
    {
        Timer++;
        if(Timer == 1)
        {
            EnclosingCircle();
        }
        if (Timer % 6 == 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float range = Main.rand.NextFloat(80, 128);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                LineStreakRenderer lineStreakRenderer = ModContent.GetInstance<LineStreakRenderer>();
                lineStreakRenderer.NewStreak(pos, 60, Projectile);
            }
        }

        if (Timer % 8 == 0)
        {
            float range = Main.rand.NextFloat(48, 64);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
            var sp = SparkleParticle.Spawn(pos, Vector2.Zero);
            sp.outerColor = Color.LightSkyBlue;
            sp.gravity = 0;
            sp.Scale *= 0.4f;
        }
        Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.6f, 0.1f);
        if (Timer >= 60)
        {
            SwitchState(ChargeState.Level_2);
        }
    }

    private void AI_ChargeLevel2()
    {
        Timer++;
        if (Timer == 1)
        {
            EnclosingCircle();

        }

        if (Timer % 6 == 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                float range = Main.rand.NextFloat(80, 128);
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
                LineStreakRenderer lineStreakRenderer = ModContent.GetInstance<LineStreakRenderer>();
                lineStreakRenderer.NewStreak(pos, 60, Projectile);
            }
        }

        Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.8f, 0.1f);
        Projectile.frame = 1;
        if (Timer >= 60)
        {
            SwitchState(ChargeState.Level_3);
        }
    }

    private void AI_ChargeLevel3()
    {
        Timer++;
        if (Timer == 1)
        {
            EnclosingCircle();
        }

        if (Timer % 6 == 0)
        {
            float range = Main.rand.NextFloat(48, 64);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(range, range);
            var sp = SirestiasSmokeParticle.Spawn(pos, Vector2.Zero);
            sp.Scale *= 0.4f;
            sp.offsetRot = Main.rand.NextFloat(0f, 3.14f); 
        }
        Projectile.scale = MathHelper.Lerp(Projectile.scale, ExtraMath.Osc(1f, 1.2f, speed: 12), 0.1f);
        Projectile.frame = 2;
    }
    private void AI_TryBreak()
    {
        if (Owner.channel)
            return;
        int numGlassShards = 0;
        float dmgPct = 0;
        switch (State)
        {
            case ChargeState.Level_1:
                numGlassShards = 3;
                dmgPct = 0.33f;
                break;
            case ChargeState.Level_2:
                numGlassShards = 6;
                dmgPct = 0.5f;
                break;
            case ChargeState.Level_3:
                numGlassShards = 12;
                dmgPct = 1f;
                break;
        }

        if (this.OwnedByLocalClient())
        {
            float damage = Projectile.damage;
            int finalDamage = (int)(damage * dmgPct); 
            for(int i = 0; i < numGlassShards; i++)
            {
                Vector2 upwardVelocity = -Vector2.UnitY;
                upwardVelocity *= 12;
                upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(75));
                upwardVelocity *= Main.rand.NextFloat(0.5f, 1f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, upwardVelocity, 
                    ModContent.ProjectileType<WintershardGlass>(), finalDamage, Projectile.knockBack, Projectile.owner, ai2: i);
            }
        }

        for(float f = 0; f < 5f; f++)
        {
            var sp2 = SirestiasSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.8f));
            sp2.color = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(1f));
            sp2.gravity = 0;
            sp2.noTileCollide = true;
            sp2.Scale *= 1f;
      //      sp2.stretchScale2 = new Vector2(1f, 0.5f);
            sp2.offsetRot = Main.rand.NextFloat(3.14f);
            sp2.noRot = true;
        }


        SoundStyle explosionSound = AssetRegistry.Sounds.Melee.MorrowExp;
        explosionSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(explosionSound, Projectile.position);

        for (int i = 0; i < 4; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }

        for (int i = 0; i < 8; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.DarkGray;
            spawnParams.scaleRange *= 0.5f;
            spawnParams.innerColor = Color.White;
            DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
        }
        for (float i = 0; i < 4; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Gray,
                outerGlowColor: Color.DarkGray,
                baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                duration: Main.rand.NextFloat(15, 25));
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }

        var fx = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.LightSkyBlue,
            outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
        fx.Scale *= 1f;
        FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
        FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
        Projectile.Kill();
    }

    private void SwitchState(ChargeState state)
    {
        if (this.OwnedByLocalClient())
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }
    }

    private void AI_HoldSpot()
    {

    }
    private void AI_HoldAnimation()
    {
        if (this.OwnedByLocalClient())
        {
            float easeIn = EasingFunction.OutExpo(GlobalTimer / 60f);
            Vector2 holdVelocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            holdVelocity *= 64 * easeIn;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, holdVelocity, 0.2f);
            Projectile.netUpdate = true;
        }

        Projectile.Center = Owner.Center + Projectile.velocity;
        //Step 1. Calculate the spot we hold from
        float rotation = (Projectile.Center - Owner.Center).ToRotation();
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        //  Owner.GetModPlayer<SwingPlayerV2>().isSwinging = true;
        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - MathHelper.ToRadians(90));// set arm position (90 degree offset since arm starts lowered)
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //Draw Ring
        _magicCircleRenderer ??= new MagicCircleRenderer(ModContent.Request<Texture2D>(Texture + "_MagicCircle"));
        Vector2 auraOffset = Projectile.velocity * 0.3f;
        Vector2 auraPos = Owner.Center + auraOffset;
        _magicCircleRenderer.DrawRing(auraPos, Projectile.velocity, 0, 1, Color.Lerp(Color.Transparent, Color.SkyBlue * 0.75f, GlobalTimer / 60f), Main.GlobalTimeWrappedHourly);

        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, auraPos);
        flareDrawer.color = Color.DarkBlue;
        flareDrawer.color.A = 0;
        flareDrawer.rotation = Projectile.velocity.ToRotation();
        flareDrawer.scale *= 0.3f;
        Main.spriteBatch.Draw(flareDrawer);

        float yOsc = ExtraMath.Osc(-4f, 4f, speed: 2f);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.rotation = 0;
        drawer.worldPosition.Y += yOsc;


        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 0.4f;
        backGlowDrawwer.worldPosition.Y += yOsc;
        Main.spriteBatch.Draw(backGlowDrawwer);


        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        spiralDrawer.color = Color.SkyBlue * 0.15f;
        spiralDrawer.color.A = 0;
        spiralDrawer.scale = Vector2.One * 0.4f * EasingFunction.OutExpo(GlobalTimer / 30f);
        spiralDrawer.worldPosition.Y += yOsc;
        spiralDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(spiralDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.5f;
        glowDrawer.color.A = 0;
        glowDrawer.scale = Vector2.Lerp(new Vector2(0.1f, 0.05f), new Vector2(0.05f, 0.1f), ExtraMath.Osc(0f, 1f, speed: 3));
        glowDrawer.worldPosition.Y += yOsc;
        Main.spriteBatch.Draw(glowDrawer);
        for (float f = 0f; f <= MathHelper.TwoPi; f += MathHelper.TwoPi / 4f)
        {
            Vector2 offset = (f + Main.GlobalTimeWrappedHourly * 3).ToRotationVector2();
            offset *= 4;
            SpritebatchDrawer drawer2 = drawer;
            drawer2.worldPosition += offset;
            drawer2.color.A = 0;
            Main.spriteBatch.Draw(drawer2);
        }
        for (float f = 0f; f <= MathHelper.TwoPi; f+=MathHelper.TwoPi / 4f)
        {
            Vector2 offset =  (f+Main.GlobalTimeWrappedHourly * 3).ToRotationVector2();
            offset *= 2;
            SpritebatchDrawer drawer2 = drawer;
            drawer2.worldPosition += offset;
            Main.spriteBatch.Draw(drawer2);
        }


        Main.spriteBatch.Draw(drawer);

        if(State == ChargeState.Level_3)
        {
            drawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 12);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
        SpritebatchDrawer glowDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        glowDrawer2.color = Color.SkyBlue * 0.25f * ExtraMath.Osc(0.5f, 1f, speed: 3);
        glowDrawer2.color.A = 0;
        glowDrawer2.scale = Vector2.One * 0.4f;
        glowDrawer2.worldPosition.Y += yOsc;
        Main.spriteBatch.Draw(glowDrawer2);


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

public class WintershardGlass : ModProjectile
{
    private enum AIState
    {
        Float,
        Blast,
        Orbit
    }

    private int _frame;
    private float _flashTimer;
    private float _randScale;
    private bool _isHoming;
    private Vector2 _targetCenter;
    private Vector2 _originalVelocity;
    private TexturedQuad _texturedQuad;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private ref float Offset => ref Projectile.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_frame);
        writer.Write(_isHoming);
        writer.WriteVector2(_targetCenter);
        writer.Write(_randScale);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _frame = reader.ReadInt32();
        _isHoming = reader.ReadBoolean();
        _targetCenter = reader.ReadVector2();
        _randScale = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 360;
        Projectile.light = 0.78f;
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    public override void AI()
    {
        base.AI();

        if (this.OwnedByLocalClient())
        {
            if(_randScale == 0f)
            {
                _frame = Main.rand.Next(4);
                _randScale = Main.rand.NextFloat(0.2f, 0.7f);
                Projectile.netUpdate = true;
            }
        }
        switch (State)
        {
            case AIState.Float:
                AI_Float();
                break;
            case AIState.Blast:
                AI_Blast();
                break;
            case AIState.Orbit:
                AI_Orbit();
                break;
        }
        if (State == AIState.Orbit)
            return;

        if (Owner.ownedProjectileCounts[ModContent.ProjectileType<WintershardWave>()] > 0)
        {
            SwitchState(AIState.Orbit);
        }
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void AI_Float()
    {

        Projectile.friendly = false;
        Projectile.velocity.X *= 0.99f;
        if (Projectile.velocity.Y < 0)
        {
            Projectile.velocity.Y += 0.2f;
            //   Projectile.velocity.Y *= 0.99f;
        }
        else
        {
 
            Projectile.velocity.Y += 0.05f;
            Projectile.velocity.Y *= 0.99f;

            Timer++;
            if (Timer >= 30)
            {
                SwitchState(AIState.Blast);
            }
        }


    }

    private void AI_Orbit()
    {
        Timer++;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        if (Timer == 1)
        {
            _originalVelocity = Projectile.velocity;
            SoundStyle fireSound = SoundID.Item9;
            fireSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, Vector2.Zero, Scale: 1.2f);
            d.noGravity = true;
        }
        float o = Offset * 7;
        float rads = (Timer+o) / 60f;
        rads *= MathHelper.TwoPi;

        float dist = 64f;
        Vector2 targetOrbitPosition = Owner.Center + Vector2.UnitY.RotatedBy(rads) * dist;
        Vector2 targetVelocity = targetOrbitPosition - Projectile.Center;

        float ease = EasingFunction.InExpo(Timer / 60f);
        Vector2 easedVelocity = Vector2.Lerp(_originalVelocity, targetVelocity, ease);
        Projectile.velocity = easedVelocity;
    }

    private void AI_Blast()
    {
        Timer++;
        Projectile.friendly = true;
        if (Timer == 60)
        {
            SoundStyle fireSound = SoundID.Item9;
            fireSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(fireSound, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, Vector2.Zero, Scale: 1.2f);
            d.noGravity = true;
        }
        if(Timer < 12)
        {
            _isHoming = true;
            _targetCenter = Projectile.Center;
        }
        if(Timer == 12)
        {
            _flashTimer = 30;
            if (this.OwnedByLocalClient())
            {
                _targetCenter = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
        }

        float speed = MathHelper.Lerp(0f, 35f, EasingFunction.InExpo(Timer / 60f));
        if (_isHoming && Timer > 13)
        {
            Vector2 targetVelocity = (_targetCenter - Projectile.Center);
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            targetVelocity *= speed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
            float dist = Vector2.Distance(Projectile.Center, _targetCenter);
            if(dist < speed * 0.5f)
            {
                _isHoming = false;
            }
        }
        else if (!_isHoming)
        {
            Projectile.tileCollide = true;
        }

    }
    public float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 0f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.LightCyan, Color.DarkBlue, completionRatio);
    }
    private void DrawPixelatedFlames(GraphicsDevice graphicsDevice)
    {
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.LightCyan * 0.5f;
        shader.InnerColor = Color.Cyan * 0.5f;
        shader.OuterColor = Color.Blue * 0.5f;
        shader.LaserTexture = TrailRegistry.SpikyTrail1;
        shader.BloomTexture = AssetManager.LaserTextures.SnowflakeLaser;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
    }


    private void DrawPixelatedPrims(GraphicsDevice graphicsDevice)
    {
        _texturedQuad ??= new TexturedQuad();

        BasicDrawingShader basicDrawing = BasicDrawingShader.Instance;
        //Here we need to prepare the shader
        float f = _frame;
        float numFrames = 4;
        Vector2 tiling = new Vector2(1f, 1f / numFrames);
        Vector2 offset = new Vector2(0, f * 1f / numFrames);
        Vector4 tilingOffset = new Vector4(offset.X, offset.Y, tiling.X, tiling.Y);
        basicDrawing.TilingOffset = tilingOffset;
        basicDrawing.RingTexture = TextureAssets.Projectile[Type];

        float radians = Main.GlobalTimeWrappedHourly * 4 + Projectile.whoAmI * 2;
        float rotation = Main.GlobalTimeWrappedHourly * 2 + Projectile.whoAmI;
        Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), radians);
        Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);
        _texturedQuad.Transform(Projectile.Center, 48 * _randScale, 48 * _randScale, rotationMatrix, rotation);
        _texturedQuad.DrawWithShader(basicDrawing);

        Color flashColor = Color.White * ExtraMath.Osc(0f, 1f, speed: 6, Projectile.whoAmI * 2);
        flashColor = Color.Lerp(flashColor, Color.Red, _flashTimer / 30f);
        flashColor.A = 0;
        _texturedQuad.SetColor(flashColor);
        _texturedQuad.DrawWithShader(basicDrawing);
    }
    private void DrawPixelated(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.35f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 0.15f;
        Main.spriteBatch.Draw(backGlowDrawwer);


        SpritebatchDrawer glowDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        glowDrawer2.color = Color.SkyBlue * 0.25f * ExtraMath.Osc(0.5f, 1f, speed: 3);
        glowDrawer2.color.A = 0;
        glowDrawer2.scale = Vector2.One * 0.2f;
    //    glowDrawer2.worldPosition.Y += yOsc;
        Main.spriteBatch.Draw(glowDrawer2);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFlames);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedPrims);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        float offset = Main.rand.NextFloat(0, MathHelper.TwoPi);
        for(float f = 0; f <= MathHelper.TwoPi; f+= MathHelper.TwoPi / 4f)
        {
            Vector2 velocity = (f+offset).ToRotationVector2();
            velocity *= 4;

            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.LightSkyBlue;
            spawnParams.outerColor = Color.DarkBlue;
            spawnParams.scaleRange *= 0.2f;
            spawnParams.gravity = 0;
            var dp = DustParticle.Spawn(Projectile.Center, velocity * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            dp.noTileCollide = true;
        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundStyle impactSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                impactSound = AssetRegistry.Sounds.Illuria.IceImpact1;
                break;
            case 1:
                impactSound = AssetRegistry.Sounds.Illuria.IceImpact2;
                break;
        }
        impactSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(impactSound, Projectile.position);
        float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
        for (float n = 0; n < 3; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.LightSkyBlue;
            spawnParams.outerColor = Color.DarkBlue;
            spawnParams.scaleRange = new Vector2(0.3f, 1f);
            DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f) * 0.3f, spawnParams);
        }

        SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
        sp.initialColor = Color.White * 0.14f;
    }

}