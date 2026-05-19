using Stellamod.Common.ArmorReforge;
using Stellamod.Common.UI;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;


namespace Stellamod.Common.ArmorReforgeSystem;

[Autoload(Side = ModSide.Client)]
public class ReforgeUISystem : BaseUISystem
{
    private Vector2 _worldPos;
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;

    public ReforgeUIState reforgeUIState;
    public static string RootTexturePath => "Stellamod/Common/ArmorReforgeSystem/";
    public float flashTimer;
    public float inTimer;
    public bool open;
    public ManagedRenderTarget UITarget => ModContent.GetInstance<UIRenderTargets>().uiTarget;
    public float InterpolationTime => 0.5f;
    public override int uiSlot => Slot_MajorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        reforgeUIState = new ReforgeUIState();
        reforgeUIState.Activate();

        On_Main.CheckMonoliths += RenderUI;
    }

    private void RenderUI(On_Main.orig_CheckMonoliths orig)
    {
        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
        {
            PlayerInput.SetZoom_UI();
            Main.spriteBatch.GraphicsDevice.SetRenderTarget(UITarget);
            Main.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                    Main.UIScaleMatrix);

            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

            Main.spriteBatch.End();
            PlayerInput.SetZoom_World();
        }

        orig();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (flashTimer > 0)
            flashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (open)
        {
            inTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            inTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(inTimer <= 0 && _userInterface?.CurrentState != null)
            {
                _userInterface.SetState(null);
            }
        }
      
        inTimer = MathHelper.Clamp(inTimer, 0f, InterpolationTime);

        float dist = Vector2.Distance(Main.LocalPlayer.position, _worldPos);
        if (dist > 160)
        {
            CloseUI();
        }

        if ((!Main.playerInventory && _userInterface.CurrentState != null) || (Main.npcShop == 1))
        {
            CloseUI();
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
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    public bool CanReforge()
    {
        Player player = Main.LocalPlayer;
        if (player.HasItem(ModContent.ItemType<GlisteningPearl>()))
        {
            return true;
        }
        return false;
    }

    public void ReforgeArmor(Player player, Item item)
    {
        List<ArmorReforgeType> armorReforges = GeneralHelpers.GetEnumList<ArmorReforgeType>();
        ArmorReforgeType chosenReforge = armorReforges[Main.rand.Next(0, armorReforges.Count)];
        //Don't ever reforge to none
        while (chosenReforge == ArmorReforgeType.None)
            chosenReforge = armorReforges[Main.rand.Next(0, armorReforges.Count)];

        ArmorReforgeGlobalItem armorReforgeGlobalItem = item.GetGlobalItem<ArmorReforgeGlobalItem>();
        armorReforgeGlobalItem.reforgeType = chosenReforge;

        item.NetStateChanged();
        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted"));

        FXUtil.ShakeCamera(Main.LocalPlayer.Center, 1024, 16f);
        string text = LangText.ArmorReforge(chosenReforge, "DisplayName") + " " + item.Name;
        int combatText = CombatText.NewText(player.getRect(), Color.White, text);
        CombatText numText = Main.combatText[combatText];
        numText.lifeTime = 60;
    }


    public void ReforgeAccessory(Player player, Item item)
    {
        List<AccessoryReforgeType> reforges = GeneralHelpers.GetEnumList<AccessoryReforgeType>();
        AccessoryReforgeType chosenReforge = reforges[Main.rand.Next(0, reforges.Count)];
        //Don't ever reforge to none
        while (chosenReforge == AccessoryReforgeType.None)
            chosenReforge = reforges[Main.rand.Next(0, reforges.Count)];

        AccessoryReforgeGlobalItem armorReforgeGlobalItem = item.GetGlobalItem<AccessoryReforgeGlobalItem>();
        armorReforgeGlobalItem.accessoryReforgeType = chosenReforge;
        item.NetStateChanged();

        string text = LangText.AccessoryReforge(chosenReforge, "DisplayName") + " " + item.Name;
        int combatText = CombatText.NewText(player.getRect(), Color.White, text);
        CombatText numText = Main.combatText[combatText];
        numText.lifeTime = 60;
    }

    public void Reforge(Player player, Item item)
    {
        //Can't reforge nothing
        if (item == null)
            return;
        //Can't reforge air
        if (item.IsAir)
            return;

        if (item.accessory)
        {
            ReforgeAccessory(player, item);
        }
        else
        {
            ReforgeArmor(player, item);
        }
    }

    public void Reforge()
    {
        Player player = Main.LocalPlayer;
        Item armorItem = reforgeUIState.ui.armorReforgeSlot.Item;
        Item accessoryItem = reforgeUIState.ui.accessoryReforgeSlot.Item;
        Reforge(player, armorItem);
        Reforge(player, accessoryItem);

        flashTimer = 1;
        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted") with { PitchVariance = 0.5f });
        FXUtil.ShakeCamera(Main.LocalPlayer.Center, 1024, 16f);
        player.RemoveItem(ModContent.ItemType<GlisteningPearl>(), 1);
    }

    public void OpenUI()
    {
        open = true;
        //Set State
        _worldPos = Main.LocalPlayer.position;
        _userInterface.SetState(reforgeUIState);
    }

    public void CloseUI()
    {
        open = false;
    }


    public override void PreSaveAndQuit()
    {
        //Calls Deactivate and drops the item
        if (_userInterface.CurrentState != null)
        {
            _userInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "LunarVeil: Reforge UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
             
                        SpriteBatch spriteBatch = Main.spriteBatch;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

                        float lerp = inTimer / InterpolationTime;
                        Vector2 offset = Vector2.Lerp(-Vector2.UnitX * 100, Vector2.Zero, EasingFunction.OutSine(lerp));
                        Color color = Color.Lerp(Color.Transparent, Color.White, lerp);
                        spriteBatch.Draw(UITarget, offset, color);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
