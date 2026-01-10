using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.ArmorsIL
{
    public class CauldrAntennaDrawLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> _antennaTextureAsset;
        // Returning true in this property makes this layer appear on the minimap player head icon.
        public override bool IsHeadLayer => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _antennaTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CauldrMothAntenna");
        }
        public override void Unload()
        {
            base.Unload();
            _antennaTextureAsset = null;
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<CauldrMothDrawPlayer>().hasCauldrMothSet;
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            // The following code draws ExampleItem's texture behind the player's head.
            var position = drawInfo.Center + new Vector2(0f, -24f) - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.
          
            SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float range = MathHelper.ToRadians(8);
            float oscRot = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, 3));
            drawInfo.DrawDataCache.Add(new DrawData(
                _antennaTextureAsset.Value,
                position, 
                null, 
                Color.White,
                oscRot,
                new Vector2(_antennaTextureAsset.Width() /2f, _antennaTextureAsset.Height()), 
                1f,
                spriteEffects, 
                0 
            ));
        }
    }
    public class CauldrPotDrawLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> _cauldronTextureAsset;
        // Returning true in this property makes this layer appear on the minimap player head icon.
        public override bool IsHeadLayer => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _cauldronTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/CauldrMothCauldron");
        }
        public override void Unload()
        {
            base.Unload();
            _cauldronTextureAsset = null;
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            // The layer will be visible only if the player is holding an ExampleItem in their hands. Or if another modder forces this layer to be visible.
            return drawInfo.drawPlayer.GetModPlayer<CauldrMothDrawPlayer>().hasCauldrMothSet;

            // If you'd like to reference another PlayerDrawLayer's visibility,
            // you can do so by getting its instance via ModContent.GetInstance<OtherDrawLayer>(), and calling GetDefaultVisibility on it
        }

        // This layer will be a 'child' of the head layer, and draw before (beneath) it.
        // If the Head layer is hidden, this layer will also be hidden.
        // If the Head layer is moved, this layer will move with it.
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);
        // If you want to make a layer which isn't a child of another layer, use `new Between(Layer1, Layer2)` to specify the position.
        // If you want to make a 'mobile' layer which can render in different locations depending on the drawInfo, use a `Multiple` position.

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            // The following code draws ExampleItem's texture behind the player's head.
            var position = drawInfo.Center + new Vector2(0f, -4) - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y); // You'll sometimes want to do this, to avoid quivering.
            position.Y += ExtraMath.Osc(0f, 4f);
            position.X -= drawInfo.drawPlayer.direction * 20;
                
            // Queues a drawing of a sprite. Do not use SpriteBatch in drawlayers!
            drawInfo.DrawDataCache.Add(new DrawData(
                _cauldronTextureAsset.Value, // The texture to render.
                position, // Position to render at.
                null, // Source rectangle.
                Color.White, // Color.
                0f, // Rotation.
                _cauldronTextureAsset.Size() * 0.5f, // Origin. Uses the texture's center.
                1f, // Scale.
                SpriteEffects.None, // SpriteEffects.
                0 // 'Layer'. This is always 0 in Terraria.
            ));
        }
    }

    public class CauldrMothDrawPlayer : ModPlayer
    {
        public bool hasCauldrMothSet;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasCauldrMothSet = false;
        }


    }

    [AutoloadEquip(EquipType.Head)]
    public class CauldrMothHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<CauldrMothHood, CauldrMothCoat, CauldrMothLegs>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == Type && body.type == ModContent.ItemType<CauldrMothCoat>() && legs.type == ModContent.ItemType<CauldrMothLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.healthBonus += 50;
            stats.accessorySlots += 1;
            stats.defenseBonus += 33;
        }

        public override void UpdateArmorSet(Player player)
        {
            base.UpdateArmorSet(player);
            CauldrMothDrawPlayer drawPlayer = player.GetModPlayer<CauldrMothDrawPlayer>();
            drawPlayer.hasCauldrMothSet = true;

            CauldronPlayer cauldronPlayer = player.GetModPlayer<CauldronPlayer>();
            int craftCount = cauldronPlayer.Crafts.Count;
            int amountToAdd = craftCount / 5;

            var stats = player.GetStats();
            stats.defenseBonus += amountToAdd;
            stats.healthBonus += amountToAdd;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class CauldrMothCoat : ModItem
    {

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.bossEndurance += 0.12f;
            stats.enemyEndurance += 0.3f;
            stats.accessorySlots += 1;
            stats.defenseBonus += 35;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class CauldrMothLegs : ModItem
    {

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.accessorySlots += 1;
            stats.defenseBonus += 10;
        }
    }
}
