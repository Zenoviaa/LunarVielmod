using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest
{
    internal class NestBossBar : ModBossBar
    {
        private int VerlibossHeadIndex = -1;
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (VerlibossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[VerlibossHeadIndex];
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            // Make the bar shake the less health the NPC has
            float lifePercent = drawParams.Life / drawParams.LifeMax;
            float shakeIntensity = Utils.Clamp(1f - lifePercent - 0.2f, 0f, 1f);
            drawParams.BarCenter.Y -= 20f;
            drawParams.BarCenter += Main.rand.NextVector2Circular(0.5f, 0.5f) * shakeIntensity * 15f;
            VerlibossHeadIndex = npc.GetBossHeadTextureIndex();
            return true;
        }
    }
}
