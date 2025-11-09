using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class TorchAmbientParticles : GlobalTile
    {
        public override void EmitParticles(int i, int j, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
        {
    
            base.EmitParticles(i, j, tileCache, typeCache, tileFrameX, tileFrameY, tileLight, visible);
            if (!TileID.Sets.Torch[tileCache.TileType])
                return;

            if (Main.rand.NextBool(16))
            {
                Vector2 worldPosition = new Vector2(i * 16, j * 16);
                Vector2 spawnPosition = worldPosition + Main.rand.NextVector2Circular(16, 16);
                Vector2 velocity = Main.rand.NextVector2Circular(0.3f, 0.3f);
                Particle.NewParticle<AmbientEmberParticle>(spawnPosition, velocity);
            }
        }
    }
}
