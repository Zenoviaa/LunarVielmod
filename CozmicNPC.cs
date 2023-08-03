
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Materials;
using Stellamod.Items.Consumables;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using Terraria.GameContent;
using System;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Trails;
using Stellamod.Items.Accessories;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;

using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.WorldG;

namespace Stellamod
{
    public class CozmicNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (EventWorld.Gintzing)
            {
                spawnRate = (int)((double)spawnRate * 0.01);
                maxSpawns = (int)((float)maxSpawns * 6.2f);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Zombie || npc.type == NPCID.BigSlimedZombie || npc.type == NPCID.TorchZombie || npc.type == NPCID.ArmedTorchZombie || npc.type == NPCID.ZombieMushroom || npc.type == NPCID.ZombieElfGirl || npc.type == NPCID.ArmedZombieSwamp || npc.type == NPCID.ArmedZombieEskimo || npc.type == NPCID.ZombieElfBeard || npc.type == NPCID.ZombieEskimo || npc.type == NPCID.ZombieDoctor || npc.type == NPCID.ZombieElf || npc.type == NPCID.ZombieMerman || npc.type == NPCID.ZombieRaincoat || npc.type == NPCID.BigZombie || npc.type == NPCID.BigFemaleZombie || npc.type == NPCID.BaldZombie || npc.type == NPCID.ArmedZombieTwiggy || npc.type == NPCID.ArmedZombieSlimed)
            {
                if (Main.bloodMoon)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrorFragments>(), 2, 0, 5));
                }
                npcLoot.Add(ItemDropRule.Common(ItemID.Leather, Main.rand.Next(1, 3)));
            }
            if (npc.type == NPCID.DemonEye || npc.type == NPCID.DemonEye2 || npc.type == NPCID.DemonEyeOwl || npc.type == NPCID.DemonEyeSpaceship)
            {
                if (Main.bloodMoon)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrorFragments>(), 2, 0, 5));
                }
            }
            if (npc.type == NPCID.GreekSkeleton)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GladiatorSpear>(), 8, 1, 1));
            }
            if (npc.type == NPCID.Harpy)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AzureFragments>(), 2, 1, 7));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloudSkaters>(), 30, 1, 1));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Wingspand>(), 30, 1, 1));
            }
            if (npc.type == NPCID.FlyingAntlion || npc.type == NPCID.GiantFlyingAntlion)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwarmerStaff>(), 30, 1, 1));
            }
            if (npc.type == NPCID.Vulture)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Tallon>(), Main.rand.Next(0, 2)));

            }


            if (npc.type == NPCID.BloodZombie || npc.type == NPCID.Drippler)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrorFragments>(), 2, 0, 5));

            }
            if (npc.type == NPCID.BloodJelly || npc.type == NPCID.BloodJelly || npc.type == NPCID.GreenJellyfish || npc.type == NPCID.PinkJellyfish)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GelatinOzze>(), Main.rand.Next(0, 2)));

            }
            if (npc.type == NPCID.SkeletronHand)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.Bone, 1, 5, 15));
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.Bone, 1, 10, 35));
            }

            if (npc.type == NPCID.Demon && NPC.downedBoss3)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Infernis>(), 15, 1, 1));
            }
            if (npc.type == NPCID.CaveBat)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheDeafen>(), 15, 1, 1));
            }
        }
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.SkeletronHead)
            {

                if (Main.GraveyardVisualIntensity <= 0.4)
                {
                    Main.GraveyardVisualIntensity += 0.02f;
                }
                else
                {
                    Main.GraveyardVisualIntensity = 0.41f;
                }
            }
            return true;
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (npc.type == NPCID.SkeletronHead || npc.type == NPCID.SkeletronHand)
            {


                NPCID.Sets.TrailingMode[npc.type] = 0;
                NPCID.Sets.TrailCacheLength[npc.type] = 15;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                var effects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + npc.Size / 2 + new Vector2(0f, npc.gfxOffY);
                    Color color = npc.GetAlpha(Color.Lerp(new Color(110, 50, 110), new Color(80, 40, 80), 1f / npc.oldPos.Length * k) * (1f - 1f / npc.oldPos.Length * k));
                    spriteBatch.Draw((Texture2D)TextureAssets.Npc[npc.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0f);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            if (npc.type == NPCID.Spazmatism)
            {
                NPCID.Sets.TrailingMode[npc.type] = 0;
                NPCID.Sets.TrailCacheLength[npc.type] = 15;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                var effects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + npc.Size / 2 + new Vector2(0f, npc.gfxOffY);
                    Color color = npc.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / npc.oldPos.Length * k) * (1f - 1f / npc.oldPos.Length * k));
                    spriteBatch.Draw((Texture2D)TextureAssets.Npc[npc.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0f);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            if (npc.type == NPCID.Retinazer)
            {
                NPCID.Sets.TrailingMode[npc.type] = 0;
                NPCID.Sets.TrailCacheLength[npc.type] = 15;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                var effects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + npc.Size / 2 + new Vector2(0f, npc.gfxOffY);
                    Color color = npc .GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / npc.oldPos.Length * k) * (1f - 1f / npc.oldPos.Length * k));
                    spriteBatch.Draw((Texture2D)TextureAssets.Npc[npc.type], drawPos, new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, npc.frame.Size() / 2, npc.scale, effects, 0f);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return true;

        }

    }
}

