using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class LittleJavelin : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToCombatTool(0.03f, 0.30f);
            Item.shoot = ModContent.ProjectileType<LittleJavelinThrow>();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankJuggler>();
        }
    }

    public class LittleJavelinThrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;

        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 15 == 0)
            {
                DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.Zero, Color.White, 0.2f);
                dp.outerColor = Color.Brown;
            }

            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                Vector2 oldDrawCenter = oldPos + Projectile.Size / 2f - Main.screenPosition;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.1f;
                spriteBatch.Draw(texture, oldDrawCenter, null, afterImageColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
