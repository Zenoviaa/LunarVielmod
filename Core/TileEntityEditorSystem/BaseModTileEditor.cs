using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal abstract class BaseModTileEditor : UIPanel
    {
        public virtual void Load(ModTileEntity modTileEntity) { }
        public virtual void Apply(ModTileEntity modTileEntity) { }
    }
}
