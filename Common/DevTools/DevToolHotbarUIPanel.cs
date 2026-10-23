using Stellamod.Common.ItemBrowser;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;

public class DevToolHotbarUIPanel : UIPanel
{
    public DevToolHotbarUIPanel()
    {
        Buttons = new();
    }
    public readonly List<DevToolHotbarButton> Buttons;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 264;
        Height.Pixels = 64;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.GodmodeIcon.Asset,
            () =>
            {
                return ((int)DevTools.godMode, 3);
            },
            () =>
            {
                var godmode = (int)DevTools.godMode;
                godmode++;
                if (godmode >= 3)
                {
                    godmode = 0;
                }
                DevTools.godMode = (GodMode)godmode;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.WingIcon.Asset,
            () =>
            {
                var frame = DevTools.infiniteFlight ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.infiniteFlight = !DevTools.infiniteFlight;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.ReachIcon.Asset,
            () =>
            {
                var frame = DevTools.infiniteReach ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.infiniteReach = !DevTools.infiniteReach;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.ItemIcon.Asset,
            () =>
            {
                var itemBrowser = ModContent.GetInstance<ItemBrowserSystem>();
                var frame = itemBrowser.IsOpen() ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                var itemBrowser = ModContent.GetInstance<ItemBrowserSystem>();
                itemBrowser.ToggleUI();
            }));



        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.CameraIcon.Asset,
            () =>
            {
                return ((int)DevTools.freeCamera, 3);
            },
            () =>
            {
                var godmode = (int)DevTools.freeCamera;
                godmode++;
                if (godmode >= 3)
                {
                    godmode = 0;
                }
                DevTools.freeCamera = (FreeCamera)godmode;
            }));
        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.SpawnsIcon.Asset,
            () =>
            {
                var frame = WorldDevTools.DisableNPCSpawns ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                WorldDevTools.ToggleNPCSpawns();
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.FreezeTimeIcon.Asset,
            () =>
            {
                var frame = WorldDevTools.FreezeTime ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                WorldDevTools.ToggleFreezeTime();
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.TimeSliderIcon.Asset,
            () =>
            {
                var frame = DevTools.showTimeSlider ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.showTimeSlider = !DevTools.showTimeSlider;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.LightHackIcon.Asset,
            () =>
            {
                var frame = DevTools.LightIntensity > 0 ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.LightIntensity += 0.25f;
                if(DevTools.LightIntensity >= 1.1f)
                {
                    DevTools.LightIntensity = 0f;
                }
            },
            () =>
            {
                return Color.Lerp(Color.Black, Color.White, DevTools.LightIntensity + 0.2f);
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.TrashIcon.Asset,
            () =>
            {
                return (0, 1);
            },
            () =>
            {
                WorldDevTools.ClearItems();
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.MinimapIcon.Asset,
            () =>
            {
                return (0, 1);
            },
            () =>
            {
                TileUtilities.UpdateMap(new Rectangle(0, 0, Main.maxTilesX, Main.maxTilesY), 255);
            }));

        foreach (var btn in Buttons)
        {
            Append(btn);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = Main.screenWidth - Width.Pixels - 432;
        Top.Pixels = 8;
        var i = 0;
        foreach (var btn in Buttons)
        {
            btn.Left.Pixels = i * 40 + 16;
            i++;
        }
        Width.Pixels = 432;
        Height.Pixels = 64;
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        base.DrawSelf(spriteBatch);
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
    }
}
