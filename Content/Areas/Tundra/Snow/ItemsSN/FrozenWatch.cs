using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.ItemsSN;

public class FrozenWatchSystem : ModSystem
{
    public double frozenTime;
    public bool isTimeFrozen;
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
