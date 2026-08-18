using Microsoft.Xna.Framework.Input;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Core.StructureSelector;
using Stellamod.Tiles;
using Stellamod.UI;
using Stellamod.WorldG.StructureManager;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ConsoleMenu;

public static class LunarDebugging
{
    public static bool clouds;
}

public abstract class ConsoleCommand : ModType
{
    protected override void Register()
    {
        ModTypeLookup<ConsoleCommand>.Register(this);
    }

    public abstract string GetCommandName();
    public abstract Arguments GetArguments();
    public abstract bool Invoke(params string[] args);
}


public class Arguments
{
    public Arguments()
    {
        potentialArguments = new();
    }
    public HashSet<string> potentialArguments;
    public Arguments next;
}

public class CloudsCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "clouds";
    }

    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        LunarDebugging.clouds = !LunarDebugging.clouds;
        return true;
    }
}
public class StyrCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "styr";
    }

    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        Vector2 tempSpawnPoint = Main.LocalPlayer.Center;
        tempSpawnPoint.Y -= 32;
        if (MultiplayerHelper.IsHost)
        {
            int npcIndex = NPC.NewNPC(new EntitySource_Misc("cutscene"), (int)tempSpawnPoint.X, (int)tempSpawnPoint.Y, ModContent.NPCType<E>(), ai1: 3);
        }
        else
        {
            MultiplayerHelper.SpawnNPCFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<E>(), (int)tempSpawnPoint.X, (int)tempSpawnPoint.Y, ai1: 3);
        }

        return true;
    }
}
public class StyrCutsceneCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "styrscene";
    }

    public override Arguments GetArguments()
    {
        return null;
    }

    public override bool Invoke(params string[] args)
    {
        return false;
    }
}
public class StructuresCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "structures";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        StructureSelectorUISystem uiSystem = ModContent.GetInstance<StructureSelectorUISystem>();
        uiSystem.ToggleUI();
        return true;
    }
}

public class LavaPlatformCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "lavaplatform";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(), (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<BigMoltenPlatform>());
        return true;
    }
}
public class LavaArenaCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "lavaarena";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        Point center = Main.LocalPlayer.Center.ToTileCoordinates();
        int width = 175;
        int height = 100;
        var bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width + 10, height + 10);

        //Not world gen idc if slow
        for (int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for (int y = bounds.topLeft.Y; y < bounds.bottomRight.Y; y++)
            {
                WorldGen.PlaceTile(x, y, ModContent.TileType<CindersparkDirt>(), mute: true, forced: true);
            }
        }

        //Not world gen idc if slow
        bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width, height);
        for(int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for(int y = bounds.topLeft.Y; y<  bounds.bottomRight.Y; y++)
            {
                WorldGen.KillTile(x, y, noItem: true);
            }
        }

        //Fill With Lava
        bounds = TileUtilities.CenterTileBoundsTileSpace(Main.LocalPlayer.Center, width, height / 2);

        for (int x = bounds.topLeft.X; x < bounds.bottomRight.X; x++)
        {
            for (int y = bounds.topLeft.Y; y < bounds.bottomRight.Y; y++)
            {
                WorldGen.PlaceLiquid(x, y + height / 2, (byte)LiquidID.Lava, 255);
            }
        }

      
        return true;
    }
}
public class UndoCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "undo";
    }
    public override Arguments GetArguments()
    {
        return null;
    }
    public override bool Invoke(params string[] args)
    {
        SnapshotSystem system = ModContent.GetInstance<SnapshotSystem>();
        system.Undo();
        return true;
    }
}
public class ResetCommand : ConsoleCommand
{
    public override string GetCommandName()
    {
        return "reset";
    }

    public override Arguments GetArguments()
    {
        Arguments arguments0 = new Arguments();
        arguments0.potentialArguments = new()
        {
            "level",
            "boss",
            "gintze"
        };

        return arguments0;
    }

    public override bool Invoke(params string[] args)
    {
        if (args.Length <= 0)
            return false;
        Player player = Main.LocalPlayer;
        switch (args[0])
        {
            case "level":

                player.GetModPlayer<LevelingPlayer>().ResetStats();
                return true;
            case "boss":
                DownedBossSystem.ResetFlags();
                DownedBossTracker.ResetFlags();
                DownedBossRewardPlayer rewardPlayer = player.GetModPlayer<DownedBossRewardPlayer>();
                rewardPlayer.ResetFlags();
                return true;
            case "gintze":
                if (MultiplayerHelper.IsHost)
                {
                    ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
                    colosseumSystem.Reset();
                }
                else
                {
                    Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.ResetColosseum).Send(-1);
                }
                return true;
        }

        return false;
    }
}
public class ConsoleUI : UIPanel
{
    private bool _enterDown;
    private UIInputTextField _textField;
    private Arguments _arguments;
    public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 64;
    public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) + 64;
    private ConsoleSystem ConsoleSystem => ModContent.GetInstance<ConsoleSystem>();
    public ConsoleUI()
    {
        _textField = new UIInputTextField("...");
        _textField.OnUpdateText += ParseText;
    }

    private void ParseText(string text)
    {
        var args = ConsoleSystem.ParseCommand(text);
        var arguments = ConsoleSystem.GetArguments(args.name, args.arguments);
        _arguments = arguments;
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        Width.Pixels = 350;
        Height.Pixels = 48;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;

        _textField.Top.Set(0, 0);
        Append(_textField);
        Orient();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        _textField.Focus();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!_textField.focused)
        {
       
            Close();
        }
        else
        {
            Main.drawingPlayerChat = false;
        }
        if (!_enterDown)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _enterDown = true;
            }
        }
        else if (_enterDown)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                _enterDown = false;
                var args = ConsoleSystem.ParseCommand(_textField.Text);
                bool e = ConsoleSystem.ExecuteCommand(args.name, args.arguments);
                string arguments = string.Empty;
                foreach (var argument in args.arguments)
                    arguments += $"{argument} ";
                if (e)
                {
              
                    Main.NewText($"Execute: {args.name} {arguments}", Color.LightGreen);
                }
                else
                {
                    Main.NewText($"Failed: {args.name} {arguments}", Color.IndianRed);
                }
                _textField.SetText(string.Empty);
                Close();
            }
        }
        Orient();
    }

    private void Orient()
    {

        Left.Pixels = RelativeLeft + 100;
        Top.Pixels = RelativeTop;
    }

    private void Close()
    {
        ModContent.GetInstance<ConsoleSystem>().CloseUI();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        /*
        Vector2 position = GetDimensions().ToRectangle().TopLeft();
        Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
        Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f);*/
        this.QuickMouseInteraction();
    }

}
public class ConsoleUIState : UIState
{
    public ConsoleUI consoleUI;
    public ConsoleUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        consoleUI = new ConsoleUI();
        Append(consoleUI);
    }
}
[Autoload(Side = ModSide.Client)]
public class ConsoleSystem : ModSystem
{
    private bool _slashDown;
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;

    public ConsoleUIState consoleUIState;
    public ConsoleCommand[] commands;
    public Dictionary<string, ConsoleCommand> commandLookup;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        consoleUIState = new();
        commands = ModContent.GetContent<ConsoleCommand>().ToArray();
        commandLookup = new Dictionary<string, ConsoleCommand>();
        foreach (ConsoleCommand command in commands)
        {
            string name = command.GetCommandName();
            commandLookup.TryAdd(name, command);
        }
    }

    public Arguments GetArguments(in string name, in string[] args)
    {
        if (!commandLookup.ContainsKey(name))
            return null;
        ConsoleCommand command = commandLookup[name];
        Arguments arguments = command.GetArguments();
        int index = 0;
        if (args.Length <= index)
            return null;
        string arg = args[index];

        //Keep moving forward until arguments don't match anymore
        while (index < args.Length && arguments.potentialArguments.Contains(arg) && arguments.next != null)
        {
            arguments = arguments.next;
            index++;
            arg = args[index];
        }
        return arguments;
    }

    public (string name, string[] arguments) ParseCommand(in string command)
    {
        string[] args = command.Split(' ');
        if (args.Length <= 0)
            return ("", new string[0]);

        List<string> arguments = new List<string>();
        for (int i = 1; i < args.Length; i++)
        {
            arguments.Add(args[i]);
        }

        string name = args[0];
        return (name, arguments.ToArray());
    }

    public bool ExecuteCommand(in string name, in string[] arguments)
    {
        if (!commandLookup.ContainsKey(name))
            return false;
        return commandLookup[name].Invoke(arguments);
    }


    public override void UpdateUI(GameTime gameTime)
    {


        if (!_slashDown)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
            {
                _slashDown = true;
            }
        }
        else if (_slashDown)
        {
            if (Keyboard.GetState().IsKeyUp(Keys.OemTilde))
            {
                _slashDown = false;
                OpenUI();
            }
        }
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
    }


    public void ToggleUI()
    {
        if (_userInterface.CurrentState != null)
        {
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();
        }
        else
        {
            SoundStyle soundStyle = SoundID.MenuOpen;
            SoundEngine.PlaySound(soundStyle);
            OpenUI();
        }
    }

    public void OpenUI()
    {
        _userInterface.SetState(consoleUIState);
    }

    public void CloseUI()
    {
        _userInterface.SetState(null);
    }

    public override void PreSaveAndQuit()
    {
        //Calls Deactivate and drops the item
        if (_userInterface.CurrentState != null)
        {
            CloseUI();
            _userInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Console UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
