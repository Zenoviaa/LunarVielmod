using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Common.UI;

[Autoload(Side = ModSide.Client)]
public class UIRenderTargets : ModSystem
{
    public ManagedRenderTarget uiTarget;
    public override void OnModLoad()
    {
        base.OnModLoad();
        uiTarget = ManagedRenderTarget.New();
    }


}
