using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Projectiles.IgniterExplosions.Stein;
using Stellamod.Projectiles.Steins;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class Hultinstein : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 7;
        Item.useTime = 8;
        Item.useAnimation = 8;
        Item.shoot = ModContent.ProjectileType<HultinsteinBarrage>();
        staminaProjectileShoot = ModContent.ProjectileType<HultFist>();
        meleeWeaponType = MeleeWeaponType.Stein;
        staminaDamageMultiplier = 2;
        staminaCost = 3;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStein>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}

public class HultinsteinBarrage : ModProjectile
{
    private Vector2 _start;
    private Vector2 _end;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 12;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_start);
        writer.WriteVector2(_end);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _start = reader.ReadVector2();
        _end = reader.ReadVector2();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override void AI()
    {
        base.AI();
     //   ProjectileID.Sets.TrailCacheLength[Type] = 8;
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                _start = Owner.Center + Main.rand.NextVector2Circular(45, 45);
                _end = MovementUtilities.SteinGetEndPoint(Owner, _start, Main.MouseWorld, maxDistance: 80);
                Projectile.netUpdate = true;
            }
        }
        if (Timer == 2)
        {

            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
            ThrustParticle ts = ThrustParticle.Spawn(Projectile.Center, Projectile.velocity);
            ts.bloomColor = Color.LightGray;
            ts.Scale *= 0.5f;
        }

        if(Timer % 8 == 0)
        {
            var ts = ThickSmokeParticle.Spawn(Projectile.Center, Vector2.Zero);
            ts.expand = true;
            ts.color *= 0.5f;
            ts.Scale *= 0.2f;
        }
        Projectile.Center = MovementUtilities.SteinCalculateSwingPoint(Timer / 12f, _start, _end);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            float ratio = (float)(i + 1) / (float)Projectile.oldPos.Length;
            SpritebatchDrawer fadeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            fadeDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            fadeDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.3f;
            Main.spriteBatch.Draw(fadeDrawer);
        }
        return false;
        
    }
}


public class HultFist : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private Vector2 _originalPosition;
    public int SwingTime = 60;
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
        // DisplayName.SetDefault("Slasher");
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
    }
    public override void SetDefaults()
    {
        Projectile.damage = 10;
        Projectile.timeLeft = SwingTime;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.height = 100;
        Projectile.width = 100;
        Projectile.friendly = true;
        Projectile.scale = 1f;
    }



    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_originalPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _originalPosition = reader.ReadVector2();
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
                player.velocity = Projectile.DirectionTo(oldMouseWorld) * 5f;
            }
        }

        if (Timer == 25)
        {
            player.itemTime = 40;
            player.itemAnimation = 40;
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

            player.GetModPlayer<DashPlayer>().DashCount += 3;
            player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;
            Bounced = true;



            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SteinHulting") { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) });
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
            for (int i = 0; i < player.GetModPlayer<MeleeEffectsPlayer>().steinWordBonus + 1; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<GREAT>(),
                    (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
            }

            float rot = player.velocity.ToRotation();
            float spread = 0.6f;
            Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);
                Dust.NewDustPerfect(Projectile.position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(255, 255, 255), 1);
                Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.LightPink * 0.5f, Main.rand.NextFloat(0.5f, 1));
            }

            target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, Projectile.knockBack);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.position.X, player.position.Y, 0, 0,
                ModContent.ProjectileType<Hulthit1>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
            for (int i = 0; i < 26; i++)
            {
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                DustParticle.Spawn(target.Center, (Vector2.One * Main.rand.Next(1, 9)).RotatedByRandom(MathHelper.TwoPi), spawnParams);
            }

            for (int i = 0; i < 12; i++)
            {
                var sp = SparkleParticle.Spawn(target.Center + Main.rand.NextVector2CircularEdge(128, 128), Vector2.Zero);
                Color color = new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255));
                sp.innerColor = color;
                sp.outerColor = Color.Lerp(color, Color.Black, 0.5f);
                sp.flickering = true;
                sp.Scale *= 0.75f;
                sp.Velocity = (sp.Center - target.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.5f);
                sp.gravity = 0;
                sp.noTileCollide = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.DeepPink, 1f).noGravity = true;
            }

            target.SimpleStrikeNPC(Projectile.damage * 2, 1, crit: false, 1);
            FXUtil.ShakeCamera(Projectile.Center, 512, 16);
        }
    }

    //     public PrimDrawer TrailDrawer { get; private set; } = null;
    public float WidthFunction(float completionRatio)
    {
        return 124 * MathHelper.SmoothStep(1f, 0f, Timer / (float)SwingTime);
    }
    public Color ColorFunction(float completionRatio)
    {
        float inRatio = completionRatio / 0.3f;
        inRatio = EasingFunction.InOutSine(inRatio);
        float outRatio = (1f - completionRatio) / 0.3f;
        outRatio = EasingFunction.InOutSine(outRatio);
        return Color.White * inRatio * outRatio;
    }

    private void DrawPixelatedTrails(GraphicsDevice gDevice)
    {
        BlackFireShader blackFireShader = BlackFireShader.Instance;
        Vector2[] array = new Vector2[64];
        for (int i = 0; i < array.Length; i++)
        {
            float ratio = (float)i / (float)array.Length;
            ref Vector2 point = ref array[i];
            point = Vector2.Lerp(_originalPosition, Projectile.Center, ratio);
        }
        blackFireShader.InnerColor = Color.White;
        blackFireShader.OuterColor = Color.LightGray;
        blackFireShader.BackColor = Color.DarkGray;
        blackFireShader.PrimaryTexture2 = TrailRegistry.LightningTrail;
        TrailDrawer.Draw(Main.spriteBatch, array, ColorFunction, WidthFunction, blackFireShader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrails);
        return false;

    }
}