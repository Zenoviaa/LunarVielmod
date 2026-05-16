using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class PerfectionStaff : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 10; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
        Item.DamageType = DamageClass.Magic;
        Item.width = 20; // hitbox width of the Item
        Item.height = 20; // hitbox height of the Item
        Item.useTime = 40; // The Item's use time in ticks (60 ticks == 1 second.)
        Item.useAnimation = 40; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.staff[Item.type] = true;
        Item.noMelee = true; //so the Item's animation doesn't do damage
        Item.knockBack = 3; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
        Item.value = 10000; // how much the Item sells for (measured in copper)
        Item.rare = ItemRarityID.Orange; // the color that the Item's name will be in-game
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1") with { PitchVariance = 0.4f }; // The sound that this Item plays when used.
        Item.shoot = ModContent.ProjectileType<PerfectionProj>();
        Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
        Item.channel = true;
        Item.mana = 18;
        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float numberProjectiles = 3;
        float rotation = MathHelper.ToRadians(14);
        position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
            Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
        }
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        //(p.ModProjectile as StaffWaveHold).MagicCircleStyle = 1;
         return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankStaff>();
    }
}


public class PerfectionProj : ModProjectile
{
    private NPC _target;
    private Vector2 _endPoint;
    private Vector2 _controlPoint1;
    private Vector2 _controlPoint2;
    private Vector2 _initialPos;
    private Vector2 _wantedEndPoint;
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_controlPoint1);
        writer.WriteVector2(_controlPoint2);
        writer.WriteVector2(_initialPos);
        writer.WriteVector2(_endPoint);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _controlPoint1 = reader.ReadVector2();
        _controlPoint2 = reader.ReadVector2();
        _initialPos = reader.ReadVector2();
        _endPoint = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.usesIDStaticNPCImmunity = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 120;
        Projectile.Size = new Vector2(12, 12);
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 200;
    }

    public override void AI()
    {
        Timer++;

        if (Timer == 1)
        {
            _initialPos = Projectile.Center;
            _endPoint = Projectile.Center;
            if (this.OwnedByLocalClient())
            {
                _controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                _controlPoint2 = _endPoint + Main.rand.NextVector2CircularEdge(1000, 1000);
                Projectile.netUpdate = true;
            }
        }


        if (Timer % 8 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Color.SpringGreen, 0.5f);
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.Scale *= 0.3f;
            sp.outerColor = Color.Turquoise;
        }
        if (Timer % 8 == 0)
        {
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald);
            d.noGravity = true;
            d.scale *= 0.6f;
        }

        float distanceSQ = float.MaxValue;
        if (_target == null || !_target.active)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if ((_target == null || npc.DistanceSQ(Projectile.Center) < distanceSQ)
                    && !npc.friendly
                    && !npc.dontTakeDamage)
                {
                    _target = npc;
                    distanceSQ = Projectile.Center.DistanceSQ(_target.Center);
                }
            }
        }

        if (_target != null && _target.DistanceSQ(Projectile.Center) < 10000000 && _target.active)
        {
            _wantedEndPoint = _initialPos - (_target.Center - _initialPos);
            if (Projectile.ai[0] < 10)
            {
                _endPoint = _wantedEndPoint;
            }
        }

        Projectile.velocity = Vector2.Zero;
       // Projectile.rotation = (Projectile.Center - ExtraMath.CubicBezier(_initialPos, _controlPoint1, _controlPoint2, _endPoint, Timer * 0.01f + 0.025f)).ToRotation() - MathHelper.PiOver2;
        _endPoint = _endPoint.MoveTowards(_wantedEndPoint, 16);
        Projectile.Center = ExtraMath.CubicBezier(_initialPos, _controlPoint1, _controlPoint2, _endPoint, Timer * 0.01f);
        if (_target == null || Timer > 200)
            Projectile.Kill();
    }



    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Turquoise * 0.5f;
        shader2.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Turquoise * 0.5f;
        bloom.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.SpringGreen, Color.DarkBlue, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(10, 2, completionRatio);
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        SpritebatchDrawer perfectDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        perfectDrawer.color *= ExtraMath.Osc(0.7f, 1f, speed: 6f, offset: Projectile.whoAmI);
        perfectDrawer.rotation = 0;
        perfectDrawer.scale *= 1.3f;
        Main.spriteBatch.Draw(perfectDrawer);

        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            perfectDrawer.worldPosition = pos;

            float ratio = (float)i / (float)Projectile.oldPos.Length;
            perfectDrawer.color = Color.Lerp(Color.Turquoise, Color.DarkBlue,ratio);
            perfectDrawer.color *= MathHelper.SmoothStep(1f, 0f, ratio) * 0.5f;
            Main.spriteBatch.Draw(perfectDrawer);

        }

        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        bloomDrawer.color = Color.Turquoise;
        bloomDrawer.color *= ExtraMath.Osc(0.7f, 1f, speed: 6f, offset: Projectile.whoAmI);
        bloomDrawer.color.A = 0;
        bloomDrawer.color *= 0.5f;
        bloomDrawer.rotation = 0;
        bloomDrawer.scale *= 0.15f;
        Main.spriteBatch.Draw(bloomDrawer);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Lighting.AddLight(Projectile.Center, Color.Turquoise.ToVector3() * 1.75f * Main.essScale);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundStyle impactSound = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon") with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(impactSound, Projectile.position);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Turquoise, Color.DarkTurquoise, 45);
        fx.Scale *= Main.rand.NextFloat(0.4f, 0.6f);
        float numDust = 4;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 vel = -Projectile.velocity;
            vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(6, 12);
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Turquoise;
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
                spawnParams.innerColor = Color.LightGreen;
                spawnParams.outerColor = Color.Turquoise;
                spawnParams.scaleRange *= 0.4f;
                var dp = DustParticle.Spawn(Projectile.oldPos[i] + Projectile.Size * 0.5f, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;

            }
        }
    }
}
