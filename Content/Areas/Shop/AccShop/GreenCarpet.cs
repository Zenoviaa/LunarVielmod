using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _frameIndex;
        private int _frameTimer;
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
            MountData.flightTimeMax = 30; // The amount of time in frames a mount can be in the state of flying.

            // Misc
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<GreenCarpetMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 60; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code

            MountData.playerHeadOffset = 22;

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
        public override void UpdateEffects(Player player)
        {
            base.UpdateEffects(player);
            MountData.flightTimeMax = 30;
            MountData.runSpeed = 6;
            player.waterWalk = true;
            player.waterWalk2 = true;
            player.gravity *= 0.5f;

         //   player.velocity.Y -= 0.1f;
            if (Main.rand.NextBool(16))
            {
                Dust.NewDustPerfect(player.Bottom, ModContent.DustType<Sparkle>());
            }
        }
 
        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Rectangle idleFrame = new Rectangle(0, 0, frame.Width, frame.Height);
            int frameCount = 60;
            Rectangle drawFrame = texture.AnimationFrame(ref _frameIndex, ref _frameTimer, 8, frameCount, true);
            drawPosition.X -= drawFrame.Width / 2;
            drawPosition.Y += ExtraMath.Osc(0f, 8);
            spriteBatch.Draw(texture, drawPosition, drawFrame, drawColor);
            return false;
        }
    }
}
