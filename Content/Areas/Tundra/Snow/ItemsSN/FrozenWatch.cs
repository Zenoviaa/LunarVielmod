using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Content.Areas.Tundra.Snow.ItemsSN;

public class FrozenWatchSystem : ModSystem
{
    public double frozenTime;
    public bool isTimeFrozen;
    public float frozenEaseTimer;

    private float WatchEaseInTime => 60;
    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(frozenTime);
        writer.Write(isTimeFrozen);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        frozenTime = reader.ReadDouble();
        isTimeFrozen = reader.ReadBoolean();
    }
    public override void PostUpdateTime()
    {
        base.PostUpdateTime();
        if (isTimeFrozen)
        {
            Main.time = frozenTime;
        }

        float direction = isTimeFrozen ? 1f : -1f;
        frozenEaseTimer += direction;
        frozenEaseTimer = MathHelper.Clamp(frozenEaseTimer, 0f, WatchEaseInTime);
    }

    public void ToggleFrozenTime()
    {
        string message = string.Empty;
        if (isTimeFrozen)
        {
            isTimeFrozen = false;
            frozenTime = 0;
            message = LangText.Common("TimeUnFrozen");
        }
        else
        {
            message = LangText.Common("TimeFrozen");
            isTimeFrozen = true;
            frozenTime = Main.time;
        }

        //Use Item is called on the server, so I believe this will toggle and sync properly?
        if (Main.netMode == NetmodeID.Server)
        {
            NetworkText txt = NetworkText.FromLiteral(message);
            ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
            NetMessage.SendData(MessageID.WorldData, -1, -1);
        }
        else
        {
            Main.NewText(message, 34, 121, 100);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        base.ModifyInterfaceLayers(layers);
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "LunarVeil: Frozen Time Icon",
                delegate
                {
                    if (frozenEaseTimer > 0)
                    {
                        float ease = EasingFunction.InOutSine(frozenEaseTimer / WatchEaseInTime);
                        Vector2 drawPos = new Vector2(48, Main.screenHeight * 0.2f);
                        SpriteBatch spriteBatch = Main.spriteBatch;
                        Asset<Texture2D> watchTextureAsset = TextureAssets.Item[ModContent.ItemType<FrozenWatch>()];
                        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(watchTextureAsset, drawPos + Main.screenPosition);
                        drawer.color = Color.Lerp(Color.Transparent, Color.White, ease);
                        drawer.scale = Vector2.Lerp(Vector2.One * 1.5f, Vector2.One, ease);
                        spriteBatch.Draw(drawer);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}

public class FrozenWatch : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Green;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.autoReuse = false;
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/TheWorld");
    }

    public override bool? UseItem(Player player)
    {
        ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
        screenShaderSystem.TintScreen(Color.Cyan, 0.5f, 60);
        FrozenWatchSystem watchSystem = ModContent.GetInstance<FrozenWatchSystem>();
        watchSystem.ToggleFrozenTime();
        return true;
    }
}
