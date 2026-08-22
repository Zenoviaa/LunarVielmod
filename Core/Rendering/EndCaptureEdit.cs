using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class EndCaptureEdit : ModSystem
{
    public override void Load()
    {
        base.Load();
        IL_Main.DoDraw += ForceEndCapture;
    }

    private void ForceEndCapture(ILContext il)
    {
        /*
        try
        {
            ILCursor c = new ILCursor(il);
            if(c.TryGotoNext(MoveType.After, i => i.MatchCallvirt<FilterManager>(nameof(FilterManager.CanCapture))))
            {
                for(int i = 0; i < 6; i++)
                    c.EmitPop();
                c.Emit(OpCodes.Brtrue_S);
            }
        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<Stellamod>(), il);
        }*/
    }
}
