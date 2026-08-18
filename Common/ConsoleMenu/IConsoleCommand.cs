using Microsoft.Xna.Framework.Input;
using Stellamod.UI;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ConsoleMenu;

public static class LunarDebugging
{
    public static bool clouds;
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
        this.QuickMouseInteraction();

        //Get all commands that could potential match this
        string[] potentialCommands = ConsoleSystem.GetMatches(_textField.Text);

        //Now draw them upwards from the text field, showing what command you might want
        for(int i = 0; i < potentialCommands.Length; i++)
        {
            ref var text = ref potentialCommands[i];
            Vector2 pos = _textField.GetDimensions().ToRectangle().TopLeft();
            pos.Y -= i * 18;
            pos.Y -= 32;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, 
                FontAssets.MouseText.Value, text, pos, Color.Lerp(Color.LightGreen, Color.LightGreen * 0.5f, ExtraMath.Osc(0f, 1f, speed: 1, i)), 0, Vector2.Zero, Vector2.One);
        }
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

    public string[] GetMatches(string command)
    {
        command = command.ToLower();
        List<string> matches = new List<string>();
        foreach(var kvp in commandLookup)
        {
            if (kvp.Key.StartsWith(command))
                matches.Add(kvp.Key);
        }
        return matches.ToArray();
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
