﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.Fenix.Projectiles;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Stellamod.Utilis;
using Stellamod.WorldG;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;


namespace Stellamod.NPCs.Event.GreenSun.Dulacrowe
{
    public class Tulacrowe : ModNPC
    {
        // States
        public enum ActionState
        {
            Asleep,
            Notice,
            Attack,
            Idle,
            Summon,
            BA,
        }
        // Current state
        public ActionState State = ActionState.Idle;

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 40;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 80; // The width of the npc's hitbox (in pixels)
            NPC.height = 118; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 12; // The amount of damage that this npc deals
            NPC.defense = 80; // The amount of defense that this npc has
            NPC.lifeMax = 4510; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
            NPC.value = 500f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 4f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;


            return (spawnInfo.Player.ZoneAcid() && EventWorld.GreenSun && NPC.downedPlantBoss) ? 0.2f : 0f;


            
        }

        public override void AI()
        {

            NPC.TargetClosest();
            switch (State)
            {
              
               
                
                
                
                case ActionState.Notice:
                    NPC.damage = 0;
                    NPC.velocity *= 0.9f;
                    counter++;
                    Notice();
                    break;
       
                case ActionState.Summon:
                    NPC.damage = 90;
                    counter++;
                    Summon();
                    NPC.velocity *= 0f;
                    break;


                case ActionState.Idle:
                    NPC.damage = 0;
                    counter++;
                    NPC.aiStyle = 44;
                    Idling();
                    break;

                default:
                    counter++;
                    break;
            }

            Vector3 RGB = new(2.30f, 2.21f, 0.72f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
        }

        public bool Tu = false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Since the NPC sprite naturally faces left, we want to flip it when its X velocity is positive
            SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            // The rectangle we specify allows us to only cycle through specific parts of the texture by defining an area inside it

            // Using a rectangle to crop a texture can be imagined like this:
            // Every rectangle has an X, a Y, a Width, and a Height
            // Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
            // Our Width and Height values specify how big of an area we want to sample starting from X and Y
            Rectangle rect;

            switch (State)
            {
                case ActionState.Asleep:
                    rect = new(0, 0, 114, 8 * 92);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 8, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.Attack:
                    rect = new Rectangle(0, 15 * 92, 114, 11 * 92);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 11, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.Idle:
                    rect = new Rectangle(0, 10 * 118, 80, 29 * 118);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2, 29, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.Summon:
                    rect = new Rectangle(0, 1 * 118, 80, 9 * 118);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 6, 9, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.Notice:
                    rect = new Rectangle(0, 1 * 118, 80, 1 * 118);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 600, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.BA:
                    rect = new Rectangle(0, 1 * 118, 80, 1 * 118);
                    spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 600, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;
            }

            if (Tu)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                    Color color = NPC.GetAlpha(Color.Lerp(new Color(9, 228, 11), new Color(9, 226, 58), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }


            return false;
        }



        int Tti = 0;
        public void Idling()
        {
            Tti++;
            Tu = false;
            if (Tti < 60)
            {

                NPC.velocity.X *= 1.03f;
            }

            // TargetClosest sets npc.target to the player.whoAmI of the closest player.
            // The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
            // This is also automatically flipped if npc.confused.
            NPC.TargetClosest(true);

            // Now we check the make sure the target is still valid and within our specified notice range (500)
            if (Tti > 540)
            {
                if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 270f)
                {
                    // Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
                    Tti = 0;
                    State = ActionState.Notice;
                    ResetTimers();
                }

            }
           
        }

        public void Notice()
        {
            timer++;
            if (timer >= 23)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), NPC.position);
                State = ActionState.Summon;
                ResetTimers();

            }

            if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 475f)
            {
                Tti = 0;
                State = ActionState.Idle;
                ResetTimers();
            }
        }

        public void Summon()
        {
            timer++;
            Tu = true;
            Player player = Main.player[NPC.target];
            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

            if (timer == 48)
            {
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                NPC.velocity *= 0f;

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GoldPrice2") { Pitch = Main.rand.NextFloat(-10f, 10f) }, NPC.Center);
                switch (Main.rand.Next(4))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RibbonStaffBoom1"), NPC.position);
                        float numberProjectiles = 20;
                        float rotation = MathHelper.ToRadians(10);

                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<TulacroweFireball>(), 40, 1, Main.myPlayer, 0, 0);
                            }

                        }

                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon"), NPC.position);
                        if (StellaMultiplayer.IsHost)
                        {
                            float speedYa = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 100, 0, speedYa * 0, ModContent.ProjectileType<TulacBombProj>(), 50, 0f, Owner: Main.myPlayer);
                        }
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RibbonStaffBoom1"), NPC.position);
                        if (StellaMultiplayer.IsHost)
                        {
                            float num = 64;
                            for (float i = 0; i < num; i++)
                            {
                                float progress = i / num;
                                float rot = MathHelper.TwoPi * progress;
                                Vector2 direction2 = Vector2.UnitY.RotatedBy(rot);
                                Vector2 velocity = direction2 * 10;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 100, velocity.X, velocity.Y, ModContent.ProjectileType<TulacroweFireball>(), 50, 0f, Owner: Main.myPlayer);
                            }
                        }
                        break;
                    case 3:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RibbonStaffBoom1"), NPC.position);
                        float numberProjectiles2 = 6;
                        float rotation2 = MathHelper.ToRadians(50);

                        for (int i = 0; i < numberProjectiles2; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Vector2 perturbedSpeed = new Vector2((direction.X * 1.5f), (direction.Y * 1.5f)).RotatedBy(MathHelper.Lerp(-rotation2, rotation2, i / (numberProjectiles2 - 1))) * 1f;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<TulacroweFireball>(), 45, 1, Main.myPlayer, 0, 0);
                            }

                        }
                        break;
                }



                NPC.netUpdate = true;
                ShakeModSystem.Shake = 4;
                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array


            }

            if (timer == 54)
            {
                // after .66 seconds, we go to the hover state. //TODO, gravity?
                State = ActionState.Idle;
                ResetTimers();
            }
        }
        public void ResetTimers()
        {
            timer = 0;
            frameCounter = 0;
            frameTick = 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Tulahal>(), 2, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheReaving>(), 3, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VirulentPlating>(), 1, 1, 15));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GlowDust>(), newColor: new Color(24, 142, 61));
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GunFlash>(), newColor: new Color(24, 142, 61));
                Main.dust[d].rotation = (Main.dust[d].position - NPC.position).ToRotation() - MathHelper.PiOver4;
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The highest form of worshippers of Gothivia corrupted and risen from the grounds of the fallen Govheil Castle")
            });
        }
    }
}