using Stellamod.Core.Foggy;
using Stellamod.Core.StructureSelector;
using Stellamod.Core.UI;
using Stellamod.Helpers;
using Stellamod.WorldG.StructureManager;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ToolsSystem
{
    public abstract class BaseToolbarButton : UIButtonIcon
    {

    }
    public class FogButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            fogSystem.doDraws = !fogSystem.doDraws;
        }
    }
    public class HitboxButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ShowHitboxes = !uiSystem.ShowHitboxes;
        }
    }
    public class StructureSelectorButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            StructureSelectorUISystem uiSystem = ModContent.GetInstance<StructureSelectorUISystem>();
            uiSystem.ToggleUI();
        }
    }
    public class TilePainterButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ToggleTilePainterUI();
        }
    }
    public class ResetBossButton : BaseToolbarButton
    {
        public override void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            base.OnButtonClick(evt, listeningElement);
            DownedBossSystem.ResetFlags();
            Main.NewText("Reset Boss Flags");
            SoundEngine.PlaySound(SoundID.AchievementComplete);
        }
    }
    public class UndoButton : BaseToolbarButton
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
