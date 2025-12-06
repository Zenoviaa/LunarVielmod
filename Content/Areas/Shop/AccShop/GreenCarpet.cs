using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class GreenCarpetMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<GreenCarpetMount>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
        }
    }
    public class GreenCarpet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ModContent.RarityType<ShopRarity>();
            Item.UseSound = SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.mountType = ModContent.MountType<GreenCarpetMount>();
        }

    }
    public class GreenCarpetMount : ModMount
    {
        // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
        // Using something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.
        protected class CarSpecificData
        {
            internal static float[] offsets = [0, 14, -14];

            internal int count; // Tracks how many balloons are still left.
            internal float[] rotations;

            public CarSpecificData()
            {
                count = 3;
                rotations = new float[count];
            }
        }

        public override void SetStaticDefaults()
        {
            // Movement
            MountData.jumpHeight = 5; // How high the mount can jump.
            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 4f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.constantJump = true; // Allows you to hold the jump button down.
            MountData.heightBoost = 20; // Height between the mount and the ground
            MountData.fallDamage = 0.5f; // Fall damage multiplier.
            MountData.runSpeed = 11f; // The speed of the mount
            MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 120; // The amount of time in frames a mount can be in the state of flying.

            // Misc
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<GreenCarpetMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 60; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 13;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;
            // Standing
            MountData.standingFrameCount = 60;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 60;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;

            // Flying
            MountData.flyingFrameCount = 60;
            MountData.flyingFrameDelay = 12;
            MountData.flyingFrameStart = 0;

            // In-air
            MountData.inAirFrameCount = 60;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
         
            // Idle
            MountData.idleFrameCount = 60;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;

            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }


        public override void SetMount(Player player, ref bool skipDust)
        {
            // When this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals for the mount.
            player.mount._mountSpecificData = new CarSpecificData();

            // This code bypasses the normal mount spawning dust and replaces it with our own visual.
            if (!Main.dedServ)
            {
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(player.Center + new Vector2(80, 0).RotatedBy(i * Math.PI * 2 / 16f), MountData.spawnDust);
                }

                skipDust = true;
            }
        }
    }
}
