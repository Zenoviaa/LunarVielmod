using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Items
{
    public abstract class ClassSwapItem :ModItem
    {
        private static bool _preReforgeSwapped;
        public bool IsSwapped { get; set; }
        public abstract DamageClass AlternateClass { get; }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!IsSwapped)
            {
                var line = new TooltipLine(Mod, "SirestiasTokenSwap", Helpers.LangText.Common("CanSwapped", AlternateClass.DisplayName));
                tooltips.Add(line);
            }
            else
            {
                var line = new TooltipLine(Mod, "SirestiasTokenSwitched", Helpers.LangText.Common("TypeSwapped"));
                tooltips.Add(line);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (IsSwapped)
            {
                DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Silver);
            }

            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture=null;
            Vector2 drawOrigin=Vector2.Zero;
            if (IsSwapped)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Token").Value;
                drawOrigin = new Vector2(20, 20);
            }
            else if(AlternateClass == DamageClass.Magic)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Magic").Value;
                drawOrigin = new Vector2(18, 18);
            } 
            else if(AlternateClass == DamageClass.Melee)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Melee").Value;
                drawOrigin = new Vector2(16, 20);
            } 
            else if (AlternateClass == DamageClass.Ranged)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Ranger").Value;
                drawOrigin = new Vector2(18, 20);
            } 
            else if (AlternateClass == DamageClass.Summon)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Summoner").Value;
                drawOrigin = new Vector2(8, 12);
            } 
            else if (AlternateClass == DamageClass.Throwing)
            {
                iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Transformer/Throwing").Value;
                drawOrigin = new Vector2(20, 20);
            }

            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

        public virtual void SetClassSwappedDefaults()
        {

        }


        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["IsSwapped"] = IsSwapped;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("IsSwapped"))
            {
                IsSwapped = tag.GetBool("IsSwapped");
                if (IsSwapped)
                {
                    Item.DamageType = AlternateClass;
                    SetClassSwappedDefaults();
                }  
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(IsSwapped);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            IsSwapped = reader.ReadBoolean();
        }

        public override void PreReforge()
        {
            base.PreReforge();
            _preReforgeSwapped = IsSwapped;
        }

        public override void PostReforge()
        {
            base.PostReforge();
            IsSwapped = _preReforgeSwapped;
            if (IsSwapped)
            {
                SetClassSwappedDefaults();
            }
        }
    }
}
