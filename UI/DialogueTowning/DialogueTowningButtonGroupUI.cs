using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.DialogueTowning;

public class DialogueTowningButtonGroupUI : UIPanel
{
    private int _index;
    private DialogueTowningButtonUI[] _buttons;
    private UIGrid _buttonsGrid;
    public int RelativeLeft => Main.screenWidth / 2;
    public int RelativeTop => Main.screenHeight - 380;

    public Vector2 offset;
    public float alpha;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 428 * 3;
        Height.Pixels = 128;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

        _buttonsGrid = new UIGrid();
        _buttonsGrid.Width.Set(0, 1f);
        _buttonsGrid.Height.Set(0, 1f);
        _buttonsGrid.HAlign = 0.5f;
        _buttonsGrid.ListPadding = 2f;

        _buttons = new DialogueTowningButtonUI[4];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = new DialogueTowningButtonUI();
            _buttonsGrid.Add(_buttons[i]);
        }
        Append(_buttonsGrid);
    }

    public void ClearButtons()
    {
        _buttonsGrid.Clear();
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
        _buttonsGrid.Add(button);
        _buttonsGrid.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        Width.Pixels = 214 * (_buttonsGrid.Count) + 32;
        Height.Pixels = 100;
        base.Update(gameTime);
        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft - Width.Pixels / 2;
        Top.Pixels = RelativeTop;
        Left.Pixels += offset.X;
        Top.Pixels += offset.Y;
        foreach (var btn in _buttons)
        {
            btn.alpha = alpha;
        }
    }
}
