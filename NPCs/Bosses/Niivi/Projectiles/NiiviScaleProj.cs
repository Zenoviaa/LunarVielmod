using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Helpers;
using Stellamod.Trails;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviScaleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.light = 0.38f;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.33f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin * 0.3f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
            return base.PreDraw(ref lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Spawn item
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2212f, 12f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                int itemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.getRect(),
                    ModContent.ItemType<IllurineScale>(), Main.rand.Next(1, 4));

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, 0, 0,
                  ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);

                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }

            return base.OnTileCollide(oldVelocity);
        }
    }
}
