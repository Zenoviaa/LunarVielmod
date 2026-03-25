using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;


public class DeadRomanceParryBuster : ModProjectile
{

    private bool _noHoming;
    private ref float Timer => ref Projectile.ai[0];
    private int Target
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
        Projectile.light = 0.78f;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Target != -1)
        {
            if (!_noHoming)
            {
                NPC target = Main.npc[Target];
                if (!target.active)
                    Target = -1;
                Vector2 targetVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                float speed = MathHelper.Lerp(0.2f, 24, EasingFunction.InOutExpo(Timer / 60f));
                targetVelocity *= speed;
                Vector2 lerpedVelocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
                Projectile.velocity = lerpedVelocity;
            }

        }
        if (Projectile.velocity.Length() < 25f)
            Projectile.velocity *= 1.05f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        _noHoming = true;
        SoundStyle busted = AssetRegistry.Sounds.Melee.ExcaliburHitBuster;
        SoundEngine.PlaySound(busted, target.Center);
    }

    private void DrawPixelatedAura(SpriteBatch sb, Vector2 sp)
    {
        float rotation = Projectile.rotation;
        SpriteEffects spriteEffects = SpriteEffects.None;

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 worldPos = pos + Projectile.Size * 0.5f;
            drawer.worldPosition = worldPos;
            drawer.rotation = Projectile.oldRot[i];
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(ratio);
            Color bladeColor = Color.Lerp(Color.Goldenrod, Color.Black, ease);
            bladeColor.A = 0;
            drawer.color = bladeColor;
            sb.Draw(drawer);
        }


        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BeamTrail;
        shader.Distortion = 0.04f;
        shader.DistortionTexture = TrailRegistry.DirnTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 0.3f;
        sb.Restart(effect: shader.Effect);
        SpritebatchDrawer glowSwordSprite = SpritebatchDrawer.FromProjectile(Projectile);
        glowSwordSprite.rotation = rotation;
        glowSwordSprite.blackIsTransparency = true;
        glowSwordSprite.color = Color.White;
        glowSwordSprite.scale = new Vector2(1f, 1f);
        sb.Draw(glowSwordSprite);

        //        glowSwordSprite.worldPosition += Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 4) * 12;
        glowSwordSprite.color = Color.Goldenrod;
        glowSwordSprite.scale *= 1.2f;
        glowSwordSprite.color *= 0.5f;
        sb.Draw(glowSwordSprite);
        sb.RestartDefaults();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedAura);
        return false;
    }
}
public class DeadRomanceParryingBlade : ModProjectile
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
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            Owner.GetModPlayer<DeadRomancePlayer>().StartParry();
            FXUtil.ShakeCamera(Projectile.Center, 1024, 2);
            SoundStyle parrySound = AssetRegistry.Sounds.Melee.ExcaliburParry;
            parrySound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(parrySound, Projectile.position);
        }

        Projectile.Center = Owner.Center;
        AI_OrientPlayer();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> projTexture = TextureAssets.Item[Owner.HeldItem.ModItem.Type];
        SpritebatchDrawer parryDrawer = SpritebatchDrawer.FromTextureAsset(projTexture, Projectile.Center);
        parryDrawer.rotation = MathHelper.ToRadians(90);
        Main.spriteBatch.Draw(parryDrawer);
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
