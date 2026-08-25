using Stellamod.Core;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
public class BlinkingStar : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override string Texture => TextureRegistry.EmptyTexture;
    //texture
    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.aiStyle = 0;
        Projectile.alpha = 255;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = 10;
        Projectile.timeLeft = 100;
        Projectile.height = 256;
        Projectile.width = 256;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        Timer -= 0.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.rotation -= 0.4f;
        Projectile.Center = Parent.Center;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_63").Value;
        Vector2 drawOrigin = texture2D4.Size() / 2f;
        Color color = Color.White;
        color.A = 0;

        float t = Timer + 5;
        float scale = 0.4f * (t + 0.2f);
        if (scale < 0)
            scale = 0;
        Main.spriteBatch.Draw(texture2D4, Parent.Center - Main.screenPosition, null, color, Projectile.rotation + Main.GlobalTimeWrappedHourly, drawOrigin, scale, SpriteEffects.None, 0f);
        return false;
    }
}