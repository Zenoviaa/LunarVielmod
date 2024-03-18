using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Stellamod.Trails
{
    public interface IDrawing
    {
        void Draw(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f);
    }
    public static class TrailHelper
    {

        public static float CalcProgress(int length, int i)
        {
            return 1f - 1f / length * i;
        }

        public static int TrailLength(this Projectile projectile)
        {
            return ProjectileID.Sets.TrailCacheLength[projectile.type];
        }
        public static void GetDrawInfo(this NPC npc, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Npc[npc.type].Value;
            offset = npc.Size / 2f;
            frame = npc.frame;
            origin = frame.Size() / 2f;
            trailLength = NPCID.Sets.TrailCacheLength[npc.type];
        }

        public static void GetDrawInfo(this Projectile projectile, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength)
        {
            texture = TextureAssets.Projectile[projectile.type].Value;
            offset = projectile.Size / 2f;
            frame = projectile.Frame();
            origin = frame.Size() / 2f;
            trailLength = NPCID.Sets.TrailCacheLength[projectile.type];

        }
        public static Rectangle Frame(this Projectile projectile)
        {
            return TextureAssets.Projectile[projectile.type].Value.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
        }

    }
}