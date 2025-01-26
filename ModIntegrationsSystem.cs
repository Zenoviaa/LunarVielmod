using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.DaedusTheDevoted;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK;
using Stellamod.NPCs.Bosses.JackTheScholar;
using Stellamod.NPCs.Bosses.Niivi;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Stellamod.NPCs.Bosses.Zui;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod
{
    // Showcases using Mod.Call of other mods to facilitate mod integration/compatibility/support
    // Mod.Call is explained here https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate
    // This only showcases one way to implement such integrations, you are free to explore your own options and other mods examples

    // You need to look for resources the mod developers provide regarding how they want you to add mod compatibility
    // This can be their homepage, workshop page, wiki, github, discord, other contacts etc.
    // If the mod is open source, you can visit its code distribution platform (usually GitHub) and look for "Call" in its Mod class
    public class ModIntegrationsSystem : ModSystem
    {
        Mod bossChecklistMod;
        public override void PostSetupContent()
        {
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();

            // We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
        }

        private void DoJackIntegration()
        {
            // The "LogBoss" method requires many parameters, defined separately below:
            // The name used for the title of the page
            string internalName = "Jack the Scholar";

            // The NPC type of the boss
            int bossType = ModContent.NPCType<JackTheScholar>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 1.2f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedJackBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.JackBossRel>(),
            };

            // The item used to summon the boss with (if available)
            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Jack/JackBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss
            // Ideally you'd have this text in the localization file, but screw that
            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

        private void DoDaedusIntegration()
        {
            string internalName2 = "DaedustheDevoted";

            // The NPC type of the boss
            int bossType2 = ModContent.NPCType<DaedusTheDevoted>();

            // Value inferred from boss progression, see the wiki for details
            float weight2 = 2.5f;

            // Used for tracking checklist progress
            Func<bool> downed2 = () => DownedBossSystem.downedDaedusBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available2 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection2 = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.DaedusBossRel>(),

            };

            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/DaedusRework/DaedusBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss
            //string spawnInfo2 = $"High at the fabled castle lies a forgotten guardian of Gothivia's ranks";
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName2,
                weight2,
                downed2,
                bossType2,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

        

    

       
        private void DoGothiviaIntegration()
        {

            string internalName8 = "GothiviatheSunGoddess";

            // The NPC type of the boss
            int bossType8 = ModContent.NPCType<NPCs.Bosses.GothiviaTheSun.GOS.GothiviaIyx>();

            // Value inferred from boss progression, see the wiki for details
            float weight8 = 18.4f;

            // Used for tracking checklist progress
            Func<bool> downed8 = () => DownedBossSystem.downedGothBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available8 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection8 = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.GothiviaBossRel>(),

            };

            // The item used to summon the boss with (if available)
            int summonItem8 = ModContent.ItemType<Items.Consumables.GothiviasSeal>();

            // Information for the player so he knows how to encounter the boss

            // The boss does not have a custom despawn message, so we omit it
            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName8,
                weight8,
                downed8,
                bossType8,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }





      

        private void DoZuiIntegration()
        {

            string internalName = nameof(ZuiTheTraveller);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<ZuiTheTraveller>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 12.9f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedZuiBoss;

            int summonItem8 = ModContent.ItemType<RadianceStone>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Zui/ZuiBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }
        private void DoStoneGolemIntegration()
        {
            string internalName = nameof(StarrVeriplant);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<StarrVeriplant>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 0.1f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedStoneGolemBoss;



            int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }



        private void DoNiiviIntegration()
        {
            string internalName = nameof(Niivi);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<Niivi>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 18.2f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedNiiviBoss;


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Niivi/NiiviPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

        private void DoRekIntegration()
        {
            string internalName = nameof(RekSnake);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<RekSnake>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 18.1f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedRekBoss;


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/REK/RekBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }


        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
            // A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it
            if (!ModLoader.TryGetMod("BossChecklist", out bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
            // Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }

            //Integrate the Bosses Here
            DoJackIntegration();
            DoDaedusIntegration();
            DoStoneGolemIntegration();
            DoZuiIntegration();
            DoNiiviIntegration();
            DoRekIntegration();
            DoGothiviaIntegration();
        }
    }
}