using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.HealthbarSystem;
using Stellamod.Content.Areas.SpecialTiles.EffectTiles;
using Stellamod.Core.TitleSystem;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public abstract class ScarletBoss : ModNPC
    {
        private Vector2 _arenaCenter;
        private float _bossHealthbarDelay;
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            DifficultyChanges.ApplyDifficultyAndScaling(NPC, numPlayers);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.boss = true;

        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            
            base.SendExtraAI(writer);
            writer.WriteVector2(_arenaCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _arenaCenter = reader.ReadVector2();
        }

        public Player MyTarget => Main.player[NPC.target];
        public float FacingDirectionToTarget => MyTarget.Center.X < NPC.Center.X ? -1 : 1;
        public int TargetDirection => (int)FacingDirectionToTarget;
        public IEntitySource SourceFromThis => NPC.GetSource_FromThis();
        public string Texture_BossIcon => Texture + "_BossIcon";
        public string Texture_BossBar => Texture + "_BossBar";

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if(_arenaCenter == Vector2.Zero)
            {
                _arenaCenter = NPC.Center;
                NPC.netUpdate = true;
            }

            BarrierBlockSystem.BossArenaCenter = _arenaCenter;
            if (Main.netMode == NetmodeID.Server)
                return;

            if (!NPC.boss)
                return;

            //Healthbar isn't going to appear instantly, so we can do funny things where the boss isn't a boss for a second
            _bossHealthbarDelay++;
            if (_bossHealthbarDelay < 15)
                return;

            ModContent.GetInstance<BossHealthbarSystem>().Add(this);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public virtual bool CanFight()
        {
            return true;
        }

        public void ShowNamePlate()
        {
            //UI can't run on the server
            if (Main.netMode == NetmodeID.Server)
                return;

            TitleCardUISystem uiSystem = ModContent.GetInstance<TitleCardUISystem>();
            uiSystem.OpenUI(DisplayName.Value, 7);
            uiSystem.titleUIState.titleCardUI.LineTexture = ModContent.Request<Texture2D>(TitleCardUISystem.RootTexturePath + "UnderlineBiome");
        }
    }
}
