using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Buffs.Scorpion;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.ScorpionMountSystem
{
    public class ScorpionSpecificData
    {
        public int legCount;
        public float[] upperLegRotations;
        public float[] lowerLegRotations;
        public BaseScorpionItem scorpionItem;
        public ScorpionSpecificData()
        {
            legCount = 6;
            upperLegRotations = new float[legCount];
            lowerLegRotations = new float[legCount];
        }
    }

    public abstract class BaseScorpionMount : ModMount
    {
        public Asset<Texture2D> chairTexture;
        public Asset<Texture2D> underseatTexture;
        public Asset<Texture2D> tailTexture;
        public Asset<Texture2D> gunTexture;
        public Asset<Texture2D> legLowerTexture;
        public Asset<Texture2D> legUpperTexture;

        public Vector2 chairDrawOrigin;
        public Vector2 underseatDrawOrigin;
        public Vector2 tailDrawOrigin;
        public Vector2 legLowerDrawOrigin;
        public Vector2 legUpperDrawOrigin;
        public Vector2 lastUnderseatDrawPosition;
        public Vector2 underseatVelocity;
        public Vector2 tailVelocity;

        public float walkSpeed;
        public float legOffset;
        public float minGlowDistance;
        public float maxGlowDistance;
        public BaseScorpionItem scorpionItem;
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
            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.

            // Misc
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<RoyalPalaceScorpionMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 4; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            MountData.xOffset = 13;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;
            // Standing
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 0;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 0;
            MountData.runningFrameDelay = 0;
            MountData.runningFrameStart = 0;
            // Flying
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            // In-air
            MountData.inAirFrameCount = 0;
            MountData.inAirFrameDelay = 0;
            MountData.inAirFrameStart = 0;
            // Idle
            MountData.idleFrameCount = 0;
            MountData.idleFrameDelay = 0;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;


            int offset = 62;
            MountData.heightBoost = offset;
            MountData.yOffset = -offset + 32;
            MountData.playerHeadOffset = offset;
            MountData.playerYOffsets[0] = offset;
            MountData.runSpeed = 7f;
            MountData.jumpHeight = 4;
            MountData.jumpSpeed = 15f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.constantJump = false;
            MountData.spawnDust = -1;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }

            chairTexture = ModContent.Request<Texture2D>(Texture + "_Chair");
            underseatTexture = ModContent.Request<Texture2D>(Texture + "_ChairUnder");
            tailTexture = ModContent.Request<Texture2D>(Texture + "_Tail");
            //            gunTexture = ModContent.Request<Texture2D>(Texture + "_Gun");
            legLowerTexture = ModContent.Request<Texture2D>(Texture + "_LegLower");
            legUpperTexture = ModContent.Request<Texture2D>(Texture + "_LegUpper");
        }

        public override void UpdateEffects(Player player)
        {



            // This code simulates some wind resistance for the balloons.
            walkSpeed = 0.025f;
            underseatVelocity -= player.velocity;
            underseatVelocity *= 0.52f;
            tailVelocity -= player.velocity * 5f;
            tailVelocity *= 0.52f;

            ScorpionPlayer scorpionPlayer = player.GetModPlayer<ScorpionPlayer>();
            Vector2 targetMountPosition = player.position + new Vector2(-54 * player.direction, 8) + tailVelocity;
            scorpionPlayer.gunMountPosition = Vector2.Lerp(scorpionPlayer.gunMountPosition, targetMountPosition, 0.5f);

            UpdateFrontLeg1(player);
            UpdateFrontLeg2(player);
            UpdateBackLeg1(player);
            UpdateBackLeg2(player);
            UpdateThirdLeg1(player);
            UpdateThirdLeg2(player);
        }

        private void UpdateFrontLeg1(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 0;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -player.position.X * walkSpeed;
            float s = MathF.Sin(x);


            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-45 * player.direction), s);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(45 * player.direction), s);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        private void UpdateFrontLeg2(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 1;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -(player.position.X + 150f) * walkSpeed;
            float s = MathF.Sin(x);

            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-45 * player.direction), s);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(45 * player.direction), s);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        private void UpdateBackLeg1(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 2;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -(player.position.X + 150f) * walkSpeed;
            float s = MathF.Sin(x);

            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(-60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-45 * player.direction), s);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(45 * player.direction), s);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        private void UpdateBackLeg2(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 3;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -(player.position.X + 10f) * walkSpeed;
            float s = MathF.Sin(x);

            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(-60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-45 * player.direction), s);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(45 * player.direction), s);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        private void UpdateThirdLeg1(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 4;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -(player.position.X + 90f) * walkSpeed;
            float s = MathF.Sin(x);

            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-25 * player.direction), s);
                tuRotation += MathHelper.ToRadians(15 * player.direction);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(25 * player.direction), s);
                tlRotation += MathHelper.ToRadians(15 * player.direction);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        private void UpdateThirdLeg2(Player player)
        {
            var scorpionData = (ScorpionSpecificData)player.mount._mountSpecificData;
            int index = 5;
            ref float upperLegRotation = ref scorpionData.upperLegRotations[index];
            ref float lowerLegRotation = ref scorpionData.lowerLegRotations[index];

            float x = -(player.position.X + 60f) * walkSpeed;
            float s = MathF.Sin(x);

            float tuRotation;
            float tlRotation;
            if (player.velocity.Y != 0f)
            {
                tuRotation = 0;
                tlRotation = MathHelper.ToRadians(-60 * player.direction);
            }
            else
            {
                tuRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-25 * player.direction), s);
                tuRotation += MathHelper.ToRadians(-15 * player.direction);
                tlRotation = MathHelper.Lerp(0, MathHelper.ToRadians(25 * player.direction), s);
                tlRotation += MathHelper.ToRadians(-15 * player.direction);
            }

            upperLegRotation = MathHelper.Lerp(upperLegRotation, tuRotation, 0.2f);
            lowerLegRotation = MathHelper.Lerp(lowerLegRotation, tlRotation, 0.2f);
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            // When this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals for the mount.
            ScorpionSpecificData scorpionData = new ScorpionSpecificData();
            scorpionData.scorpionItem = scorpionItem;
            player.mount._mountSpecificData = scorpionData;

            //Draw Origins
            chairDrawOrigin = new Vector2(64, 34);
            underseatDrawOrigin = new Vector2(74, 0);
            tailDrawOrigin = new Vector2(96, 12);
            legUpperDrawOrigin = new Vector2(2, 29);
            legLowerDrawOrigin = new Vector2(10, 10);

            legOffset = 62;
            minGlowDistance = 7f;
            maxGlowDistance = 9f;
        }

        private void DrawChair(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var origin = chairDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = chairTexture.Size().X - origin.X;
            }
            Vector2 chairDrawPosition = drawPosition;
            chairDrawPosition.Y += VectorHelper.Osc(0f, -12f);
            DrawData drawData = new DrawData(chairTexture.Value, chairDrawPosition, null, drawColor, rotation, origin, drawScale, spriteEffects, 0);

            Glow(playerDrawData, drawData);
            playerDrawData.Add(drawData);
        }

        private void DrawUnderchair(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var origin = underseatDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = underseatTexture.Size().X - origin.X;
            }
            Vector2 underseatDrawPosition = drawPosition;
            underseatDrawPosition += new Vector2(0, 32);


            Vector2 newPosition = Vector2.Lerp(lastUnderseatDrawPosition, underseatDrawPosition, 0.01f);
            playerDrawData.Add(new DrawData(underseatTexture.Value, underseatDrawPosition + underseatVelocity, null, drawColor, rotation, origin, drawScale, spriteEffects, 0));
            lastUnderseatDrawPosition = newPosition;
        }
        private void DrawTail(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var origin = tailDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = tailTexture.Size().X - origin.X;
            }
            Vector2 finalDrawPosition = drawPosition;
            //finalDrawPosition += new Vector2(0, 42);

            DrawData tailDrawData = new DrawData(tailTexture.Value, finalDrawPosition + tailVelocity, null, drawColor, rotation, origin, drawScale, spriteEffects, 0);
            playerDrawData.Add(tailDrawData);
        }

        private void DrawLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(0, legOffset);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor,
                rotation + upperLegRotation, origin, drawScale, spriteEffects, 0));


            origin = legLowerDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }


            float extraOffset = 0;
            if (drawPlayer.direction == -1)
            {
                extraOffset += 90;
            }

            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition +
                (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor,
                rotation + lowerLegRotation, origin, drawScale, spriteEffects, 0);
            playerDrawData.Add(drawData);
        }
        private void DrawBackLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(0, legOffset - 10f);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor.MultiplyRGB(Color.Gray),
                rotation + upperLegRotation, origin, drawScale, spriteEffects, 0));


            origin = legLowerDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }



            float extraOffset = 0;
            if (drawPlayer.direction == -1)
            {
                extraOffset += 90;
            }
            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition + (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor.MultiplyRGB(Color.Gray),
                rotation + lowerLegRotation, origin, drawScale, spriteEffects, 0);
            playerDrawData.Add(drawData);
        }
        private void DrawRearLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;

            SpriteEffects drawEffects = spriteEffects;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawEffects = SpriteEffects.None;
            }
            else
            {
                drawEffects = SpriteEffects.FlipHorizontally;
            }

            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(-42 * drawPlayer.direction, legOffset);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor,
                rotation + upperLegRotation, origin, drawScale, drawEffects, 0));


            origin = legLowerDrawOrigin;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }

            float extraOffset = 0;
            if (drawPlayer.direction == 1)
            {
                extraOffset += 90;
            }
            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition
                + (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor,
                rotation + lowerLegRotation, origin, drawScale, drawEffects, 0);
            playerDrawData.Add(drawData);
        }
        private void DrawRearBackLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;
            SpriteEffects drawEffects = spriteEffects;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawEffects = SpriteEffects.None;
            }
            else
            {
                drawEffects = SpriteEffects.FlipHorizontally;
            }

            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(-42 * drawPlayer.direction, legOffset - 10f);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor.MultiplyRGB(Color.Gray),
                rotation + upperLegRotation, origin, drawScale, drawEffects, 0));


            origin = legLowerDrawOrigin;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }

            float extraOffset = 0;
            if (drawPlayer.direction == 1)
            {
                extraOffset += 90;
            }
            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition + (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor.MultiplyRGB(Color.Gray),
                rotation + lowerLegRotation, origin, drawScale, drawEffects, 0);
            playerDrawData.Add(drawData);
        }

        private void Glow(List<DrawData> playerDrawData, DrawData baseDrawData)
        {
            for (float f = 0f; f < 1f; f += 0.2f)
            {
                DrawData glowData = baseDrawData;
                glowData.position += (f * MathHelper.ToRadians(360 + Main.GlobalTimeWrappedHourly * 15f)).ToRotationVector2() * VectorHelper.Osc(minGlowDistance, maxGlowDistance);
                glowData.color *= 0.2f;
                playerDrawData.Add(glowData);
            }
        }

        private void DrawRearThirdLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;
            SpriteEffects drawEffects = spriteEffects;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawEffects = SpriteEffects.None;
            }
            else
            {
                drawEffects = SpriteEffects.FlipHorizontally;
            }

            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(-24 * drawPlayer.direction, legOffset - 5f);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor.MultiplyRGB(Color.LightGray),
                rotation + upperLegRotation, origin, drawScale, drawEffects, 0));


            origin = legLowerDrawOrigin;
            if (drawEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }

            float extraOffset = 0;
            if (drawPlayer.direction == 1)
            {
                extraOffset += 90;
            }

            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition + (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor.MultiplyRGB(Color.Gray),
                rotation + lowerLegRotation, origin, drawScale, drawEffects, 0);
            playerDrawData.Add(drawData);
        }
        private void DrawThirdLeg(int index, List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

            var origin = legUpperDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legUpperTexture.Size().X - origin.X;
            }

            float upperLegRotation = scorpionData.upperLegRotations[index];
            float lowerLegRotation = scorpionData.lowerLegRotations[index];
            Vector2 finalDrawPosition = drawPosition;
            finalDrawPosition += new Vector2(-24 * drawPlayer.direction, legOffset - 5f);
            finalDrawPosition += underseatVelocity;
            playerDrawData.Add(new DrawData(legUpperTexture.Value, finalDrawPosition, null, drawColor,
                rotation + upperLegRotation, origin, drawScale, spriteEffects, 0));

            origin = legLowerDrawOrigin;
            if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                origin.X = legLowerTexture.Size().X - origin.X;
            }

            float extraOffset = 0;
            if (drawPlayer.direction == -1)
            {
                extraOffset += 90;
            }
            DrawData drawData = new DrawData(legLowerTexture.Value, finalDrawPosition + (upperLegRotation - MathHelper.ToRadians(45 + extraOffset)).ToRotationVector2() * 32, null, drawColor,
                rotation + lowerLegRotation, origin, drawScale, spriteEffects, 0);

            playerDrawData.Add(drawData);
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            // Draw is called for each mount texture we provide, so we check drawType to avoid duplicate draws.
            if (drawType == 0)
            {
                // We draw some extra balloons before _Back texture
                var scorpionData = (ScorpionSpecificData)drawPlayer.mount._mountSpecificData;

                //Step 1 Draw Chair
                DrawTail(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);

                DrawBackLeg(1, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
                DrawRearBackLeg(3, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
                DrawRearThirdLeg(5, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);


                DrawChair(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
                DrawUnderchair(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);

                DrawLeg(0, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
                DrawRearLeg(2, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
                DrawThirdLeg(4, playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
            }
            //    glowTexture = texture;

            // by returning true, the regular drawing will still happen.
            return true;
        }

    }
}
