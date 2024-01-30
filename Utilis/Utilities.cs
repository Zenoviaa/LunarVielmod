
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Utilis
{
    public static class Utilities
    {
        public static MyPlayer GetVoidPlayer(this Player player) => player.GetModPlayer<MyPlayer>();

        public static int NewProjectileBetter(float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, int owner = -1, float ai0 = 0f, float ai1 = 0f)
        {
            if (owner == -1)
                owner = Main.myPlayer;
            damage = (int)(damage * 0.5);
            if (Main.expertMode)
                damage = (int)(damage * 0.5);
            int index = Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1);
            if (index >= 0 && index < Main.maxProjectiles)
            {
                Main.projectile[index].netUpdate = true;
            }

            return index;
        }        public static bool CircularCollision(Vector2 checkPosition, Rectangle hitbox, float radius)
        {
            float dist1 = Vector2.Distance(checkPosition, hitbox.TopLeft());
            float dist2 = Vector2.Distance(checkPosition, hitbox.TopRight());
            float dist3 = Vector2.Distance(checkPosition, hitbox.BottomLeft());
            float dist4 = Vector2.Distance(checkPosition, hitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= radius;
        }
        public static void CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix)
        {
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix zoomScaleMatrix = Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Viewport viewport = Main.instance.GraphicsDevice.Viewport;
            int width = viewport.Width;
            int height = viewport.Height;
            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            viewMatrix *= Matrix.CreateTranslation(0f, -height, 0f);
            viewMatrix *= Matrix.CreateRotationZ((float)Math.PI);
            if (Main.LocalPlayer.gravDir == -1f)
            {
                viewMatrix *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, height, 0f);
            }
            viewMatrix *= zoomScaleMatrix;
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0f, width * zoom.X, 0f, height * zoom.Y, 0f, 1f) * zoomScaleMatrix;
        }
    }

    public static class ExtensionMethods
    {
        public static bool ZoneFable(this Player player) => player.InModBiome<FableBiome>();
        public static bool ZoneGovheil(this Player player) => player.InModBiome<GovheilCastle>();
        public static bool ZoneAbyss(this Player player) => player.InModBiome<AbyssBiome>();
        public static bool ZoneAcid(this Player player) => player.InModBiome<AcidBiome>();
        public static bool ZoneAurelus(this Player player) => player.InModBiome<AurelusBiome>();
        public static bool ZoneXixianVillage(this Player player) => player.InModBiome<XixVillage>();
    }
}
