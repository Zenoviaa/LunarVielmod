using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.TriggersSystem
{
    public abstract class Trigger
    {
        public int id;
        public Point position;
        public abstract bool ShouldInvoke();
        public abstract void Invoke();
        public abstract void Edit();
        public abstract void DrawHandles(SpriteBatch spriteBatch);
    }

    public interface INetData
    {
        public void NetSend(BinaryWriter writer);
        public void NetReceive(BinaryReader reader);
    }

    public interface ISaveData
    {
        /// <summary>
        /// Allows you to save custom data for this tile entity.
        /// <br/>
        /// <br/><b>NOTE:</b> The provided tag is always empty by default, and is provided as an argument only for the sake of convenience and optimization.
        /// <br/><b>NOTE:</b> Try to only save data that isn't default values.
        /// </summary>
        /// <param name="tag"> The TagCompound to save data into. Note that this is always empty by default, and is provided as an argument only for the sake of convenience and optimization. </param>
        public void SaveData(TagCompound tag);

        /// <summary>
        /// Allows you to load custom data that you have saved for this tile entity.
        /// <br/><b>Try to write defensive loading code that won't crash if something's missing.</b>
        /// </summary>
        /// <param name="tag"> The TagCompound to load data from. </param>
        public void LoadData(TagCompound tag);

    }
    public enum TriggerID
    {
        None = 0,
        NPCSpawnTrigger = 1,
        BossSpawnTrigger = 2
    }

    public class TriggerWand : ModItem
    {
        private int _triggerToPlace;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Point16 point = new Point16(x, y);
            if (player.altFunctionUse == 2)
            {
                //Right click 
                //Cycle
                _triggerToPlace++;
                int max = Enum.GetNames<TriggerID>().Length;
                if (_triggerToPlace >= max)
                {
                    _triggerToPlace = 0;
                }
                TriggerID trigger = (TriggerID)_triggerToPlace;
                Main.NewText($"{_triggerToPlace} " + trigger.ToString(), Color.White);
            }
            else
            {
                TriggerManager triggerManager = ModContent.GetInstance<TriggerManager>();
                if (triggerManager.TryGetTrigger(new Point(x, y), out Trigger triggerToEdit))
                {
                    triggerToEdit.Edit();
                    Main.NewText($"Editing Trigger {triggerToEdit}");
                }
                else
                {
                    TriggerID triggerIdToPlace = (TriggerID)_triggerToPlace;
                    Trigger trigger = TriggerFactory.Create(triggerIdToPlace);
                    triggerManager.PlaceTrigger(new Point(x, y), trigger);
                    Main.NewText($"Placed Trigger {triggerIdToPlace} at {x} , {y}");
                }

            }
            return true;
        }
    }
    public class TriggerEraser : ModItem
    {
        private int _triggerToPlace;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            Point16 point = new Point16(x, y);
            TriggerManager triggerManager = ModContent.GetInstance<TriggerManager>();
            triggerManager.RemoveTrigger(new Point(x, y));

            return true;
        }
    }

    public class TriggerManager : ModSystem
    {
        private Dictionary<Point, Trigger> _triggerIndex;
        public bool debugTriggers;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _triggerIndex = new Dictionary<Point, Trigger>();
            debugTriggers = true;
            On_Main.DrawDust += DrawDebugHook;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawDust -= DrawDebugHook;
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            foreach (var kvp in _triggerIndex)
            {
                Trigger trigger = kvp.Value;
                if (trigger.ShouldInvoke())
                {
                    trigger.Invoke();
                }
            }
        }

        public void RemoveTrigger(Point point)
        {
            if (!_triggerIndex.ContainsKey(point))
                return;

            _triggerIndex.Remove(point);
        }

        public bool TryGetTrigger(Point point, out Trigger trigger)
        {
            if (!_triggerIndex.ContainsKey(point))
            {
                trigger = null;
                return false;
            }

            trigger = _triggerIndex[point];
            return true;
        }
        public void PlaceTrigger(Point point, Trigger trigger)
        {
            if (_triggerIndex.ContainsKey(point))
            {
                trigger.position = point;
                _triggerIndex[point] = trigger;
            }
            else
            {
                trigger.position = point;
                _triggerIndex.Add(point, trigger);
            }
        }


        private void DrawDebugHook(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (!debugTriggers)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
               null, Main.GameViewMatrix.TransformationMatrix);
            DrawDebug(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            foreach (var kvp in _triggerIndex)
            {
                Point drawPoint = kvp.Key;
                Vector2 worldPosition = drawPoint.ToWorldCoordinates();
                Vector2 drawPosition = worldPosition - Main.screenPosition;
                spriteBatch.DrawCircle(drawPosition, 16, 16, Color.Red);

                kvp.Value.DrawHandles(spriteBatch);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            int index = 0;
            tag["triggercount"] = _triggerIndex.Count;
            foreach (var kvp in _triggerIndex)
            {
                string key = $"trigger{index}";
                string keyId = key + "_id";
                string keyPosition = key + "_position";

                TagCompound saveCompound = new TagCompound();
                if (kvp.Value is ISaveData saveData)
                {
                    saveData.SaveData(saveCompound);
                }

                int id = kvp.Value.id;
                tag[keyId] = id;
                tag[keyPosition] = kvp.Key;
                tag[key] = saveCompound;
                index++;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            _triggerIndex.Clear();
            if (!tag.ContainsKey("triggercount"))
                return;

            int triggerCount = tag.GetInt("triggercount");
            for (int i = 0; i < triggerCount; i++)
            {
                string key = $"trigger{i}";
                string keyId = key + "_id";
                string keyPosition = key + "_position";

                TagCompound loadCompound = tag.GetCompound(key);
                int id = tag.GetInt(keyId);
                Point position = tag.Get<Point>(keyPosition);

                Trigger trigger = TriggerFactory.Create((TriggerID)id);

                trigger.position = position;
                if (trigger is ISaveData saveData)
                {
                    saveData.LoadData(loadCompound);
                }
                PlaceTrigger(position, trigger);
            }
        }
    }
}
