using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class LeviathanElectricRock : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Frame => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                Frame = Main.rand.Next(3);
                Projectile.netUpdate = true;
            }

        }
        if (Timer >= 20)
        {
            Projectile.hostile = true;
        }

        if (Timer % 12 == 0)
        {
            var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                ModContent.DustType<SeafloorRockDust>());
            Main.dust[d].noGravity = true;
        }

        if (Timer % 16 == 0)
        {
            Vector2 velocity = (Projectile.position - Projectile.oldPosition);
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var bp = BubbleParticle.Spawn(pos, -velocity * 0.25f);
            bp.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
            bp.gravity = 0;
        }

        if (Timer % 30 == 0)
        {
            var z = ElectricZapParticle.Spawn(
                Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
            z.Scale *= 0.5f;
        }

        Vector2 targetVelocity = (Parent.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 15;
        Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 1f);

        float distanceToParent = Vector2.Distance(Projectile.Center, Parent.Center);
        if (distanceToParent <= 32f)
            Projectile.Kill();

        Projectile.frame = (int)Frame;
        Projectile.rotation += 0.025f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");

        float distanceToParent = Vector2.Distance(Projectile.Center, Parent.Center);
        SpritebatchDrawer spriteDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        spriteDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InOutSine(Timer / 30f));
        spriteDrawer.scale *= MathHelper.Lerp(0f, 1f, EasingFunction.Clamp(distanceToParent / 384));
        Main.spriteBatch.Draw(spriteDrawer);

        spriteDrawer.texture = _outlineTextureAsset.Value;
        spriteDrawer.color = Projectile.hostile ? Color.Red : Color.Yellow;
        Main.spriteBatch.Draw(spriteDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 16; f++)
        {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                ModContent.DustType<SeafloorRockDust>(), Main.rand.NextVector2Circular(16, 16), Scale: 1);
            d.noGravity = true;
        }
    }
}
