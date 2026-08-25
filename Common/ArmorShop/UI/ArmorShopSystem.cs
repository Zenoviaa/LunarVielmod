using SteelSeries.GameSense;
using Stellamod.Common.ClassReworkSystem.AmmoRework.UI;
using Stellamod.Common.UI;
using Stellamod.Core.Tooltips;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Stellamod.Common.ArmorShop.UI.BannerShop;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Common.ArmorShop.UI;




[Autoload(Side = ModSide.Client)]
public class ArmorShopSystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    public BannerShop bannerShop;
    public override int uiSlot => Slot_MajorUI;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        BannerShopParameters armorBannerShopParameters = new BannerShopParameters();
        armorBannerShopParameters.AvailableItemsFunction = () =>
        {
            List<Item> items = new List<Item>();
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            foreach (var set in groups.Armors)
            {
                items.Add(set.heads[0]);
            }
            return items.ToArray();
        };

        armorBannerShopParameters.SelectItemFunction = (Item item) =>
        {
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            ArmorShopSet armorSet = groups.FindSet(item);
            if (!armorSet.CanPurchase())
                return;

            Player player = Main.LocalPlayer;

            if (!armorSet.HasPurchased())
            {
                player.RemoveItem(armorSet.material.type, armorSet.material.stack);
            }

            armorSet.QuickSpawn(player);
            SoundEngine.PlaySound(SoundID.Coins);
        };

        armorBannerShopParameters.SelectedItemFunction = (Item item) => false;
        armorBannerShopParameters.ViewItemFunction = (Item item) => true;
        armorBannerShopParameters.TitleKey = "ArmorShop";
        armorBannerShopParameters.TooltipKey = "ArmorShopHelp";
        armorBannerShopParameters.DrawFunction = (SpriteBatch spriteBatch, Item head, BannerDrawParameters drawParameters) =>
        {
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            ArmorShopSet armorSet = groups.FindSet(head);
            float pieceCount = armorSet.pieces.Count;
            float index = 0;
            Color originColor = drawParameters.color;
            if (!armorSet.HasPurchased())
                drawParameters.color = drawParameters.color.MultiplyRGB(Color.Black);
            foreach(var piece in armorSet.pieces)
            {
                float ratio = index / pieceCount;
                Vector2 start = drawParameters.position;
                start.Y -= 42;
                Vector2 end = drawParameters.position;
                end.Y += 42;
                Vector2 iconCenterPos = Vector2.Lerp(start, end, ratio);
                ItemSlot.DrawItemIcon(piece, ItemSlot.Context.BankItem, spriteBatch, iconCenterPos, drawParameters.scale, 32, drawParameters.color);
                index++;
            }

            //Draw the material the armor needs
            Item material = armorSet.material;
            ItemSlot.DrawItemIcon(material, ItemSlot.Context.BankItem, spriteBatch, drawParameters.position + new Vector2(32), drawParameters.scale, 32, originColor);
            
            //Draw the cost of the armor
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, material.stack.ToString(),
                drawParameters.position + new Vector2(32), originColor, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
        };
        armorBannerShopParameters.HoverTooltipFunction = (Item head) =>
        {
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            ArmorShopSet armorSet = groups.FindSet(head);
            if (armorSet.HasPurchased())
            {
                Main.HoverItem = head;
                Main.hoverItemName = head.Name;
                return;
            }

            List<TooltipLine> lines = new List<TooltipLine>();
            TooltipLine armorTypeLine = new TooltipLine(Stellamod.Instance, "ArmorType", LangText.Armor(head, "Log"));
            armorTypeLine.OverrideColor = Color.Goldenrod;
            lines.Add(armorTypeLine);
            TooltipLine materialLine = new TooltipLine(Stellamod.Instance, "ArmorMaterial", armorSet.material.Name);
            materialLine.OverrideColor = Color.White;
            lines.Add(materialLine);

            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
            renderer.SetTooltipsToDraw(lines, 64, 16);
        };

        bannerShop = new BannerShop(armorBannerShopParameters, CloseThis);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
    }

    public override void CloseThis()
    {
        base.CloseThis();
        CloseUI();
    }

    public void ToggleUI()
    {
        if (_userInterface.CurrentState != null)
        {
            bannerShop.shopMenuUIState.isOpen = false;
            /*
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();*/
        }
        else
        {
            bannerShop.shopMenuUIState.isOpen = true;
            SoundStyle soundStyle = SoundID.MenuOpen;
            SoundEngine.PlaySound(soundStyle);
            OpenUI();
        }
    }

    public void OpenUI()
    {
        //Set State
        TakeSlot();
        _userInterface.SetState(bannerShop.shopMenuUIState);
    }

    public void CloseUI()
    {
        ClearSlot();
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
                "Stellamod: Armor Shop UI",
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
