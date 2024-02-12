using Stellamod.Items.Quest.Zui;
using Stellamod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Helpers;
using Terraria.ID;

namespace Stellamod.Projectiles
{
    internal class CarianArrow : ModProjectile
    {
        private bool _foundStatue;
        private Vector2 _found;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 42;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.HeldItem.type != ModContent.ItemType<TornCarianPage>())
            {

            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (!_foundStatue && Main.myPlayer == Projectile.owner)
            {
                _found = Vector2.Zero;
                for (int x = 0; x < Main.tile.Width; x++)
                {
                    for (int y = 0; y < Main.tile.Height; y++)
                    {
                        if (Main.tile[x, y].TileType == ModContent.TileType<Rallad>())
                        {
                            _found = new Vector2(x, y).ToWorldCoordinates();
                            _foundStatue = true;
                            break;
                        }
                    }

                    if (_foundStatue)
                    {
                        break;
                    }
                }
            }

            float offset = 80 + VectorHelper.Osc(-16, 16);
            Vector2 directionToRallad = owner.Center.DirectionTo(_found);
            Projectile.Center = owner.Center + directionToRallad * offset;
            Projectile.rotation = directionToRallad.ToRotation();

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Violet, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
