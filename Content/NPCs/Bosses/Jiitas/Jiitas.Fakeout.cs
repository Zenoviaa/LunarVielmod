using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float FakeOutStartupTime
        {
            get
            {
                return 25;
            }
        }
        public float FakeOutGhostTime
        {
            get
            {
                return 45;
            }
        }
        public float FakeOutPunishTime
        {
            get
            {
                return 30;
            }
        }
        public int FakeOutBombCount
        {
            get
            {
                return 10;
            }
        }
        public float FakeOutWaitTime
        {
            get
            {
                return 300;
            }
        }
        public int FakeOutCloneCount
        {
            get
            {
                return 32;
            }
        }
        public int FakeOutDamage
        {
            get
            {
                return 30;
            }
        }

        private bool HasBeenHit;
        private float CloneAlpha;
        private Vector2 NoiseScroll;
        private List<Vector2> AfterImagePositions;
        private void AI_Fakeout()
        {
            switch (ActionStep)
            {
                case 0:
                    AI_FakeUp();
                    break;
                case 1:
                    AI_FakeStartup();
                    break;
                case 2:
                    AI_WaitForCloneHit();
                    break;
                case 3:
                    AI_ClonePunish();
                    break;
                case 4:
                    AI_ClonePullup();
                    break;
            }
        }

        private void AI_FakeUp()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasSit;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }
            NPC.TargetClosest();
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            NPC.noGravity = true;
            NPC.velocity.Y -= 0.02f;
            PlayAnimation(AnimationState.Dragup);
            if(Timer >= FakeOutStartupTime)
            {
                AfterImagePositions?.Clear();
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_FakeStartup()
        {
            Timer++;
            PrimaryDrawAlpha -= 0.02f;
            if (PrimaryDrawAlpha <= 0f)
                PrimaryDrawAlpha = 0f;
            
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            NPC.noGravity = true;
            NPC.velocity.Y -= 0.2f;
            if(NPC.velocity.Length() > 2)
            {
                NPC.velocity *= 0.9f;
            }
            Warn();
            PlayAnimation(AnimationState.Death);
            if(Timer >= FakeOutGhostTime)
            {
                NoWarn();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 teleportSpot = new Vector2();
                    teleportSpot.Y = Target.Bottom.Y - 96;
                    if (Main.rand.NextBool(2))
                    {
                        teleportSpot.X = Target.Center.X - 384;
                    }
                    else
                    {
                        teleportSpot.X = Target.Center.X + 384;
                    }
                    TeleportPos = teleportSpot;
                    NPC.netUpdate = true;
                }
           

                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_WaitForCloneHit()
        {
            Timer++;
            if (Timer == 1)
            {
                HasBeenHit = false;
            }
            if (Timer == 1)
            {
                SoundStyle jiitasSummon = AssetRegistry.Sounds.Jiitas.JiitasSummon;
                jiitasSummon.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSummon, NPC.position);
            }
            ShowClones();
            _noise ??= new FastNoiseLite();
            AfterImagePositions ??= new List<Vector2>();
            if(AfterImagePositions.Count <= 0)
            {
                for (int n = 0; n < FakeOutCloneCount; n++)
                {
                    AfterImagePositions.Add(Main.rand.NextVector2Circular(768, 256));
                }
            }

            NoiseScroll += new Vector2(-0.5f, 0f);
            if (Empowered)
            {
                NPC.velocity.X = Target.velocity.X * 0.75f;
            }

            NPC.velocity *= 0.5f;
            NPC.rotation *= 0f;
            NoWarn();
            if(Timer >= FakeOutWaitTime)
            {
                Timer = 0;
                ActionStep++;
            }
            if (HasBeenHit)
            {
                Timer = 0;
                ActionStep += 2;
            }
        }

        private void AI_ClonePunish()
        {
            Timer++;

            NPC.noGravity = true;

            int modular = (int)(FakeOutPunishTime / FakeOutBombCount);
            if(Timer % modular == 0)
            {
                Vector2 left = Target.Center - new Vector2(384, 0);
                Vector2 right = Target.Center + new Vector2(384, 0);
                float interpolant = Timer / 30f;
                Vector2 spot = Vector2.Lerp(left, right, interpolant);
                if (MultiplayerHelper.IsHost)
                {
                    spot.Y = Target.Center.Y - 500;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spot, Vector2.Zero, ModContent.ProjectileType<JiitasBomb>(), FakeOutDamage, 1, Main.myPlayer);
                }
            }
            PlayAnimation(AnimationState.Laugh);
            if(Timer >= FakeOutPunishTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_ClonePullup()
        {
            PlayAnimation(AnimationState.Situp);
            ShouldDealContactDamage = false;
            HideClones();
            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.02f;
            NPC.noGravity = true;
            Empowered = false;
            if (Timer >= SitupTime)
            {
                SwitchState(ActionState.Idle);
            }
        }

        private void DrawClones(Color drawColor)
        {
            if (_noise == null)
                return;

            if (AfterImagePositions.Count <= 0)
                return;

            if (CloneAlpha <= 0f)
                return;

            for(int n = 0; n < AfterImagePositions.Count; n++)
            {
                Vector2 afterImageOffset = AfterImagePositions[n];
                Vector2 position = NPC.Center + afterImageOffset;

                Vector2 sampleSpot = afterImageOffset + NoiseScroll;
                float noiseSample = _noise.GetNoise(sampleSpot.X, sampleSpot.Y);
                DrawCharacter(position, drawColor * CloneAlpha * noiseSample);
                afterImageOffset = afterImageOffset.RotatedBy(0.01f);
            }
        }

        private void ShowClones()
        {
            CloneDraw = true;
        }

        private void HideClones()
        {
            CloneDraw = false;
        }
    }
}
