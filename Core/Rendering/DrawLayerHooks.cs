using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

[Autoload(Side = ModSide.Client)]
public class DrawLayerHooks : ModSystem
{
    /// <summary>
    /// Draws over water, the sprite batch does not need to be restarted.
    /// </summary>
    public static readonly Queue<Action<SpriteBatch>> OverWaterDrawActions = new();
    public override void Load()
    {
        base.Load();
        On_Main.DrawInfernoRings += DrawOverWater;
    }

    private void DrawOverWater(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if(OverWaterDrawActions.Count > 0)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            while(OverWaterDrawActions.Count > 0)
            {
                OverWaterDrawActions.Dequeue().Invoke(spriteBatch);
            }
        }
    }
}
