using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;



namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class RazorWingDash : ModProjectile,
    IDrawToRenderTarget
{
    private float _framer = 0;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 12;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 48; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 225;
        Projectile.height = 225;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.timeLeft = 60;
        Projectile.localNPCHitCooldown = 6;
        Projectile.usesLocalNPCImmunity = true;
    }


    public void UpdateFrame(float speed, int minFrame, int maxFrame)
    {
        _framer += speed;
        if (_framer < minFrame)
        {
            _framer = minFrame;
        }
        if (_framer > maxFrame)
        {
            _framer = minFrame;
        }
    }

    public override void AI()
    {
        Timer++;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.Center = Parent.Center;
        UpdateFrame(0.6f, 1, 60);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothExplode") with { PitchVariance = 0.75f }, Projectile.Center);
        float numDirections = 8;
        if (MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY * 2400, 
                ModContent.ProjectileType<GothinTorch>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: numDirections, ai2: 2);
        }
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 16f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPosition = Parent.Center - Main.screenPosition;

        Rectangle rectangle = new Rectangle(0, 0, 225, 225);
        rectangle.X = ((int)_framer % 5) * rectangle.Width;
        rectangle.Y = (((int)_framer - ((int)_framer % 5)) / 5) * rectangle.Height;

        Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
        SpriteBatch spriteBatch = Main.spriteBatch;
        float drawRotation = 0;
        float drawScale = 1.4f;

        spriteBatch.Draw(texture, drawPosition,
           rectangle,
            Color.White, drawRotation, origin, drawScale, SpriteEffects.None, 0f);

        return false;
    }

    private Color GetDiscAuraColor()
    {
        return Color.Lerp(Color.Orange, Color.Aquamarine, ExtraMath.Osc(0f, 1f, speed: 24));
    }
    private Color GetDiscAuraColor2()
    {
        return Color.DarkGoldenrod;
    }

    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(120, 96, completionRatio);
    }

    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }

    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        Color secondaryLerp = Color.Lerp(GetDiscAuraColor2(), Color.Black, completionRatio);
        return Color.Lerp(GetDiscAuraColor(), secondaryLerp, completionRatio);
    }

    private Color GetSpiralDashTrailColor2(float completionRatio)
    {
        Color secondaryLerp = Color.Lerp(GetDiscAuraColor2(), Color.Black, completionRatio);
        return Color.Lerp(GetDiscAuraColor2(), secondaryLerp, completionRatio);
    }

    private void DrawRippingTrail(GraphicsDevice gDevice)
    {
        Color primaryColor = GetDiscAuraColor();
        Color darkerColor = Color.Lerp(GetDiscAuraColor2(), Color.Black, 0f);
        BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
        bloomShader.Time = Main.GlobalTimeWrappedHourly * 50;
        bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
        bloomShader.InnerColor = primaryColor;
        bloomShader.OuterColor = darkerColor;
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = GetDiscAuraColor2();
        basicLaserShader.OuterColor = GetDiscAuraColor2();
        TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor2, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawRippingTrail);
    }
}
