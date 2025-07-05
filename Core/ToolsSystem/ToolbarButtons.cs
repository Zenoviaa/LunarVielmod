using Stellamod.Core.Foggy;
using Stellamod.Core.Helpers;
using Stellamod.Core.StructureSelector;
using Stellamod.Core.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ToolsSystem
{
    internal abstract class BaseToolbarButton : UIButtonIcon
    {

    }
    internal class FogButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            fogSystem.doDraws = !fogSystem.doDraws;
        }
    }
    internal class HitboxButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ShowHitboxes = !uiSystem.ShowHitboxes;
        }
    }
    internal class StructureSelectorButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            StructureSelectorUISystem uiSystem = ModContent.GetInstance<StructureSelectorUISystem>();
            uiSystem.ToggleUI();
        }
    }
    internal class TilePainterButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ToggleTilePainterUI();
        }
    }
    internal class ResetBossButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            DownedBossSystem.ResetFlags();
            Main.NewText("Reset Boss Flags");
            SoundEngine.PlaySound(SoundID.AchievementComplete);
        }
    }
    internal class UndoButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            SnapshotSystem system = ModContent.GetInstance<SnapshotSystem>();
            system.Undo();
            // We can do stuff in here!
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}
