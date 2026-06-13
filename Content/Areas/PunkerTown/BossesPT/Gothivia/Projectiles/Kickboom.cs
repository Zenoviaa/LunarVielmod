using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class Kickboom : ModProjectile,
    IDrawToRenderTarget
{
    private Asset<Texture2D> _greenBoomTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Variant => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 10;
    }

    public override void SetDefaults()
    {
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 40;
        Projectile.localNPCHitCooldown = 6;
        Projectile.usesLocalNPCImmunity = true;
    }

    float trueFrame = 0;
    public void UpdateFrame(float speed, int minFrame, int maxFrame)
    {
        trueFrame += speed;
        if (trueFrame < minFrame)
        {
            trueFrame = minFrame;
        }
        if (trueFrame > maxFrame)
        {
            trueFrame = minFrame;
        }
    }

    public override void AI()
    {
        Timer++;
        if(Timer == 1)
        {
            ShakeScreenPosition.Shake = 3;
            FXUtil.CreateRipple(Projectile.Center);
            for(float f = 0; f < 12; f++)
            {
                Vector2 vel = Projectile.velocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(45)).SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(5f, 100);
                var dp = DustParticle.Spawn(Projectile.Center, vel);
                dp.gravity = 0.05f;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
                dp.Scale *= 0.6f;
            }
        }


        if (Timer > 29)
            Projectile.hostile = false;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.Center = Parent.Center;

        //Lighting
        Vector3 RGB = new(2.89f, 2.53f, 2.0f);
        Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        UpdateFrame(1f, 1, 50);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _greenBoomTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Alt");
        SpriteBatch spriteBatch = Main.spriteBatch;
        Asset<Texture2D> textureAsset = Variant == 1 ? _greenBoomTextureAsset : TextureAssets.Projectile[Type];

     
        Vector2 drawPosition = Parent.Center - Main.screenPosition;

        Rectangle rectangle = new Rectangle(0, 0, 518, 518);
        rectangle.X = ((int)trueFrame % 5) * rectangle.Width;
        rectangle.Y = (((int)trueFrame - ((int)trueFrame % 5)) / 5) * rectangle.Height;

        Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
   
        float drawRotation = Projectile.rotation;
        float drawScale = 2f;

        spriteBatch.Draw(textureAsset.Value, drawPosition,
           rectangle,
            (Color)GetAlpha(lightColor)!, drawRotation, origin, drawScale, SpriteEffects.None, 0f);
        
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.White, Color.Black, EasingFunction.OutExpo(Timer / 40f));
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    public void DrawToRenderTargets()
    {

    }
}
