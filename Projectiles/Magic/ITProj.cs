using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic;

public class ITProj : ModProjectile
{
    bool Moved;
    float WhiteTimer;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }
    public override void SetDefaults()
    {
        Projectile.penetrate = 5;
        Projectile.width = 17;
        Projectile.height = 16;
        Projectile.timeLeft = 860;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Projectile.velocity *= .96f;
        Projectile.ai[1]++;
        if (!Moved && Projectile.ai[1] >= 0)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/IrradiatedNest_Fall"), Projectile.position);
            Projectile.spriteDirection = Projectile.direction;
            Moved = true;
        }
        if (Projectile.ai[1] == 30)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Pitch = 0.8f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] == 60)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Pitch = 0.9f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] == 90)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Pitch = 1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] >= 120)
        {
            Projectile.Kill();
            WhiteTimer = 1;
        }

        if (Projectile.ai[1] >= 90)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 1f).noGravity = true;
            }
        }
        WhiteTimer = MathHelper.Lerp(WhiteTimer, 0, 0.1f);
        Rectangle myRect = Projectile.getRect();
        foreach (var p in Main.ActiveProjectiles)
        {
            if (p.type != ModContent.ProjectileType<ITExplosionProj>())
                continue;
            if (p == Projectile)
                continue;
            Rectangle otherRect = p.getRect();
            if (Projectile.Colliding(myRect, otherRect))
            {
                if (Projectile.ai[1] <= 100)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ITPrimer"), Projectile.position);
                    Projectile.ai[1] = 111;
                }
            }
        }

        Projectile.spriteDirection = Projectile.direction;
    }
    public override void OnKill(int timeLeft)
    {
        var entitySource = Projectile.GetSource_Death();
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<ITExplosionProj>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<IrradiatedBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
        }

        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);

        int S1 = Main.rand.Next(0, 3);
        if (S1 == 0)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb1"), Projectile.position);
        }
        if (S1 == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb2"), Projectile.position);
        }
        if (S1 == 2)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb3"), Projectile.position);
        }
        FXUtil.ShakeCamera(Projectile.Center, 2048, 8);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 drawCenter = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            sbDrawer.worldPosition = drawCenter;
            sbDrawer.color = Color.Lerp(Color.Green, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            Main.spriteBatch.Draw(sbDrawer);
        }
        SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer2);
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        Lighting.AddLight(Projectile.Center, Color.DarkSeaGreen.ToVector3() * 1.75f * Main.essScale);
        string glowTexture = Texture + "_White";
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>(glowTexture), Projectile.Center);
        //Lerping
        float progress = WhiteTimer;
        Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
        drawer.color = drawColor;
        Main.spriteBatch.Draw(drawer);
    }
}


