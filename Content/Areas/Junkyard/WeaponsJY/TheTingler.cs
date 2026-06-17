using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY;

public class TheTingler : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 28;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 50;
        Item.height = 24;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(0, 15, 0, 0);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock") with { PitchVariance = 0.75f };
        Item.autoReuse = true;
        Item.shootSpeed = 19f;
        Item.shoot = ModContent.ProjectileType<CogNeedle>();
        Item.noMelee = true;
        Item.noUseGraphic = true;
        muzzleOrigin = new Vector2(51, 14);

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 16;
    }

    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.GunShot(player, source, position, velocity, type, damage, knockback);
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        type = ModContent.ProjectileType<CogNeedle>();
    }
    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
 
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        base.ShootEffects(position, velocity);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}

public class CogNeedle : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float HitCount => ref Projectile.ai[1];
    private int _targetNpc = -1;
    private Vector2 _targetOffset;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_targetOffset);
        writer.Write(_targetNpc);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetOffset = reader.ReadVector2();
        _targetNpc = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 8;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.light = 0.5f;
    }

    public override void AI()
    {
        Timer++;
        if (Timer < 4 && Main.rand.NextBool(2))
        {
            var smokeParticle = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f));
            smokeParticle.fadeToColor = Color.Black * 0.3f;
            smokeParticle.color = Color.RosyBrown;
            smokeParticle.Scale *= 0.3f;
        }

        if (_targetNpc != -1)
        {
            NPC target = Main.npc[_targetNpc];
            if (!target.active)
            {
                Projectile.Kill();
            }

            Vector2 targetPos = target.position - _targetOffset;
            Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
            float dist = Vector2.Distance(Projectile.position, targetPos);
            Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
        }
        else
        {
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Bleeding, 180);
        target.AddBuff(BuffID.Poisoned, 180);
        if (_targetNpc == -1)
        {
            _targetNpc = target.whoAmI;
            _targetOffset = (target.position - Projectile.position) + new Vector2(0.001f, 0.001f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.netUpdate = true;
        }
        HitCount++;
        if(HitCount > 5)
        {
            Projectile.Kill();
        }
    }
    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float progress)
        {
            return MathHelper.SmoothStep(5, 0, progress);
        }

        float GetTrailWidth2(float progress)
        {
            return GetTrailWidth(progress) * 1.6f;
        }

        Color GetTrailColor(float progress)
        {
            Color inColor = Color.White;
            Color trailColor = Color.Lerp(Color.OrangeRed, Color.DarkRed, progress);
            Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
            return easeColor * 2;
        }

        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Yellow;
        shader2.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Yellow;
        bloom.OuterColor = Color.Red;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, alpha: 0.3f);
        SpritebatchDrawer mainSprite = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(mainSprite);


        mainSprite.VerticalFrame(1, Main.projFrames[Type]);
        mainSprite.color = Color.Lerp(Color.Red, Color.Transparent, EasingFunction.InOutSine(Timer / 30f));
        Main.spriteBatch.Draw(mainSprite);

        mainSprite.VerticalFrame(2, Main.projFrames[Type]);
        mainSprite.color = Color.Lerp(Color.Yellow, Color.Yellow * 0.5f, ExtraMath.Osc(0f, 1f, speed: 16)) * 0.3f;
        Main.spriteBatch.Draw(mainSprite);
        return false;
    }
    public void DrawToRenderTargets()
    {
        if (_targetNpc != -1)
            return;

        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
    }
    public override void OnKill(int timeLeft)
    {

    }
}
