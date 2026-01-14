using Stellamod.Helpers;
using Stellamod.WorldG;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace Stellamod.Content.Areas.Collosseum.Event.Common
{
    public class ColosseumWallColorEdit : ModSystem
    {
        private float _factor;
        private float _wallTimer;
        public static bool darkenLights;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Lighting.GetCornerColors += OverrideCornerColor;
            On_Lighting.GetColor_int_int += OverrideLightingColor;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Lighting.GetCornerColors -= OverrideCornerColor;
            On_Lighting.GetColor_int_int -= OverrideLightingColor;
        }

        private void OverrideCornerColor(On_Lighting.orig_GetCornerColors orig, int centerX, int centerY, out VertexColors vertices, float scale)
        {
            bool shouldDarken = darkenLights && (_wallTimer > -0);

            if (shouldDarken)
            {
                orig(centerX, centerY, out vertices, scale);
                Color darkenColor = Color.Lerp(Color.White, Color.Black, _factor * 0.55f);
                Main.instance.WallsRenderer.LerpVertexColorsWithColor(ref vertices, darkenColor, 0.65f);

                return;
            }
            orig(centerX, centerY, out vertices, scale);
        }

        public override void PostUpdateDusts()
        {
  
            base.PostUpdateDusts();
            if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
            {
                _wallTimer++;

            }
            else
            {
                _wallTimer--;
            }
            float time = 60f;
            _wallTimer = MathHelper.Clamp(_wallTimer, 0f, time);
            _factor = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_wallTimer / time));
        }
        private Color OverrideLightingColor(On_Lighting.orig_GetColor_int_int orig, int x, int y)
        {
            bool shouldDarken = darkenLights && (_wallTimer > 0 || !Main.tileSolid[Main.tile[x, y].TileType]);

            if (shouldDarken)
            {
                Color color = orig(x, y);
                Color darkenColor = Color.Lerp(Color.White, Color.Black, _factor * 0.55f);
                return color.MultiplyRGB(darkenColor);
            }
            return orig(x, y);
        }


    }
    public class ColosseumTileColor : GlobalTile
    {
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            ColosseumWallColorEdit.darkenLights = true;
            return base.PreDraw(i, j, type, spriteBatch);
        }
        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, type, spriteBatch);
            ColosseumWallColorEdit.darkenLights = false;
        }
    }
    public class ColosseumWallColor : GlobalWall
    {
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            ColosseumWallColorEdit.darkenLights = true;
            return true;

        }
        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            ColosseumWallColorEdit.darkenLights = false;
            base.PostDraw(i, j, type, spriteBatch);

        }
    }

    public class ColosseumSystem : ModSystem
    {
        public float spawnTimer;
        public bool completedBronzeColosseum;
        public bool completedSilverColosseum;
        public bool completedGoldColosseum;
        public bool completedTrueColosseum;
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(completedBronzeColosseum);
            writer.Write(completedSilverColosseum);
            writer.Write(completedGoldColosseum);
            writer.Write(completedTrueColosseum);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            completedBronzeColosseum = reader.ReadBoolean();
            completedSilverColosseum = reader.ReadBoolean();
            completedGoldColosseum = reader.ReadBoolean();
            completedTrueColosseum = reader.ReadBoolean();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["bronze"] = completedBronzeColosseum;
            tag["silver"] = completedSilverColosseum;
            tag["gold"] = completedGoldColosseum;
            tag["true"] = completedTrueColosseum;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            completedBronzeColosseum = tag.GetBool("bronze");
            completedSilverColosseum = tag.GetBool("silver");
            completedGoldColosseum = tag.GetBool("gold");
            completedTrueColosseum = tag.GetBool("true");
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (!MultiplayerHelper.IsHost)
                return;

            if (NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>()))
            {
                spawnTimer = 0;
                return;
            }

            spawnTimer++;
            if (spawnTimer < 120)
            {
                return;
            }

            Vector2 GongSpawnWorld = ColosseumWaveManager.GongSpawnWorld;
            if (!completedBronzeColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<BronzeGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<BronzeGong>());
                }
            }
            else if (!completedSilverColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<SilverGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<SilverGong>());
                }
            }
            else if (!completedGoldColosseum)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<GoldGong>()))
                {
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y, ModContent.NPCType<GoldGong>());
                }
            }
        }

        public void Reset()
        {
            completedBronzeColosseum = false;
            completedSilverColosseum = false;
            completedGoldColosseum = false;
            completedTrueColosseum = false;
        }
    }
}
