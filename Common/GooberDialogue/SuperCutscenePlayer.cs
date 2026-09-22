using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.GooberDialogue;

public class SuperCutscenePlayer : ModPlayer
{
    private bool _useLastFrame;
    public bool ShouldProgressCutscene
    {
        get;
        set;
    }

    /// <summary>
    /// The amount of ticks before the player can progress the cutscene again
    /// </summary>
    public float ProgressDelay
    {
        get;
        private set;
    }

    public override void PostUpdate()
    {
        base.PostUpdate();

        //Only the host can progress the cutscene, shall we go test this?
        if (Main.myPlayer == Player.whoAmI)
        {
            if (Player.IsCutsceneHost)
            {
                ProgressDelay--;
                if (!_useLastFrame && Player.controlUseItem && ProgressDelay <= 0)
                {
                    ProgressDelay = 15;
                    CutsceneHandler.Next();
                }
            }
        }
        _useLastFrame = Player.controlUseItem;
    }
}

public static class CutscenePlayerExtensions
{
    extension(Player player)
    {
        public bool ShouldProgressCutscene
        {
            get => player.GetModPlayer<SuperCutscenePlayer>().ShouldProgressCutscene;
            set => player.GetModPlayer<SuperCutscenePlayer>().ShouldProgressCutscene = value;
        }
        public bool IsCutsceneHost
        {
            get => player.whoAmI == CutsceneHandler.CutsceneHostWhoAmI;
        }
    }
}

