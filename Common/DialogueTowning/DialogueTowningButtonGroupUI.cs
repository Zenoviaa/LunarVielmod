using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.DialogueTowning;

public class DialogueTowningButtonGroupUI : UIPanel
{
    private int _index;
    private DialogueTowningButtonUI[] _buttons;
    public int RelativeLeft => Main.screenWidth / 2;
    public int RelativeTop => Main.screenHeight - 300;

    public Vector2 offset;
    public float alpha;
    public SpeechBoxTalkingParameters Parameters
    {
        set
        {
            foreach(var btn in _buttons)
            {
                btn.talkingParameters = value;
            }
        }
    }
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 428 * 3;
        Height.Pixels = 154;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

        _buttons = new DialogueTowningButtonUI[4];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = new DialogueTowningButtonUI();
            Append(_buttons[i]);
        }
    }

    public void ClearButtons()
    {
        _index = 0;
        for(int i =0; i < _buttons.Length; i++)
        {
            _buttons[i].realText = string.Empty;
            _buttons[i].onClickEvent = null;
        }
    }

    public void AddButton(string text, Action btn)
    {
        DialogueTowningButtonUI button = _buttons[_index];
        button.onClickEvent = btn;
        button.realText = LangText.TownDialogue(text);
        button.alpha = 0;

        _buttons[_index++] = button;
    }

    public override void Update(GameTime gameTime)
    {
 
        Width.Pixels = 214 * (_buttons.Length) + 32;
        Height.Pixels = 512;
        base.Update(gameTime);

        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft - Width.Pixels / 2;
        Top.Pixels = RelativeTop;
        Left.Pixels += offset.X;
        Top.Pixels += offset.Y;
        Top.Pixels -= 42;
        var totalWidth = _index * 212;
        float offset2 = Width.Pixels / 2 - totalWidth / 2;

        int index = 0;
        foreach (var btn in _buttons)
        {
            btn.alpha = alpha;
  
            btn.Left.Pixels = index * 212;
            btn.Left.Pixels += offset2;
            btn.Top.Pixels = ExtraMath.Osc(0, 6, speed: 2, offset: index);

            if (index >= _index)
            {
                btn.alpha = 0;
                btn.Left.Pixels += 9999999999;
            }
            
            index++;
        }
    }
}
