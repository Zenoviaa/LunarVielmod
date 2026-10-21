using Stellamod.Common.Particles;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.LostItems;

public class FabledWitchsBroomBuff : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.FabledWitchsBroomBuff.KEY;
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
        Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.mount.SetMount(ModContent.MountType<FabledWitchsBroomMount>(), player);
        player.buffTime[buffIndex] = 10; // reset buff time
    }
}

public class FabledWitchsBroom : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.FabledWitchsBroom.KEY;
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 30;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item79; // What sound should play when using the item
        Item.noMelee = true; // this item doesn't do any melee damage
        Item.mountType = ModContent.MountType<FabledWitchsBroomMount>();
    }

}

// This mount is a car with wheels which behaves similarly to the unicorn mount. The car has 3 balloons attached to the back.
public class FabledWitchsBroomMount : ModMount
{
    public override string Texture => AssetReferences.Assets.Textures.Item.FabledWitchsBroomMount.KEY;
    // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
    // Using something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.
    protected class CarSpecificData
    {
        public float flyHeight;
        public CarSpecificData()
        {

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
        MountData.fallDamage = 0F; // Fall damage multiplier.
        MountData.runSpeed = 11f; // The speed of the mount
        MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
        MountData.flightTimeMax = 9999; // The amount of time in frames a mount can be in the state of flying.
        MountData.usesHover = true;

        // Misc
        MountData.fatigueMax = 0;
        MountData.buff = ModContent.BuffType<FabledWitchsBroomBuff>(); // The ID number of the buff assigned to the mount.

        // Effects
        MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.

        // Frame data and player offsets
        MountData.totalFrames = 4; // Amount of animation frames for the mount
        MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code

        MountData.playerHeadOffset = 22;

        MountData.flightTimeMax = 9999;
        MountData.runSpeed = 5;
        MountData.acceleration = 0.19f;

        MountData.idleFrameCount = 1;
        MountData.idleFrameDelay = 12;
        MountData.idleFrameStart = 0;

        MountData.inAirFrameCount = 4;
        MountData.inAirFrameDelay = 0;
        MountData.inAirFrameStart = 0;
        if (!Main.dedServ)
        {
            MountData.textureWidth = MountData.backTexture.Width();
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
        MountData.runSpeed = 3f;
        MountData.acceleration = 0.35f;
        player.waterWalk = true;
        player.waterWalk2 = true;
        MountData.idleFrameCount = 1;
        MountData.idleFrameLoop = false;

        MountData.runningFrameCount = 4;
        MountData.runningFrameDelay = 24;
        MountData.runningFrameStart = 0;
        MountData.inAirFrameCount = 4;
        MountData.inAirFrameDelay = (int)MathHelper.Lerp(15, 4, MathF.Abs(player.velocity.X) / 8f);
        MountData.inAirFrameStart = 0;
        if (Main.rand.NextBool(4))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = player.MountedCenter + Main.rand.NextVector2Circular(32, 32) + new Vector2(player.direction * 20, 4),
                innerColor = Color.Orange.ToVector4(),
                outerColor = Color.DarkOrange.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 0.8f))
            });
        }

        var floor = TileUtilities.FallToSolidTileOrPlatform(player.Center);
        var diffY = MathF.Abs(floor.Y - player.position.Y);
        if (diffY > 128)
        {
            player.velocity.Y = MathHelper.Lerp(player.velocity.Y, 4f, 0.1f);
        }
        else
        {
            player.velocity.Y *= 0.9f;
        }

        player.fullRotation = player.velocity.X * 0.025f;
        Lighting.AddLight(player.MountedCenter + new Vector2(player.direction * 20, 4), new Vector3(0.6f, 0.6f, 0));
    }

    public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
    {
        if (drawType == 0)
        {
            var position = drawPosition;
            position.X += drawPlayer.direction * 14;
            position.Y += 8;
            Rectangle? srcRect = null;
            float drawRotation = rotation + ExtraMath.Osc(-0.1f, 0.1f, speed: 2);
            var origin = new Vector2(7, 0);
            var lanterns = AssetReferences.Assets.Textures.Item.FabledWitchsBroomMount_Lanterns.Asset;
            playerDrawData.Add(new DrawData(lanterns.Value, position, srcRect, drawColor, drawRotation, origin, drawScale, spriteEffects, 0));

            var glowingColor = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 2)) * 0.5f;
            glowingColor.A = 0;
            playerDrawData.Add(new DrawData(lanterns.Value, position, srcRect, glowingColor, drawRotation, origin, drawScale, spriteEffects, 0));

            var glowBall = AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset;
            glowingColor = Color.Lerp(Color.Transparent, Color.DarkGoldenrod, ExtraMath.Osc(0.5f, 1f, speed: 2)) * 1f;
            glowingColor.A = 0;
            position.Y += 8;
            playerDrawData.Add(new DrawData(glowBall.Value, position, srcRect, glowingColor, drawRotation, glowBall.Size() * 0.5f, drawScale * 0.15f, spriteEffects, 0));

            /*
            Vector2 pos = drawPosition;
            void DrawGlowingBallReal(SpriteBatch sb, Vector2 sp)
            {
                DrawUtilities.DrawBasicGlow(sb, pos + new Vector2(drawPlayer.direction * 6, 20).RotatedBy(drawRotation) + Main.screenPosition, 0.2f, Color.Goldenrod * 0.25f);
            }
            PixelationManager.QueueSpritebatchDrawAction(DrawGlowingBallReal, DrawLayer.OverPlayers);*/
        }
        drawPosition.Y += 6;

        // by returning true, the regular drawing will still happen.
        return true;
    }
}
