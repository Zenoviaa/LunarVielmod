using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Content.Dialogue;
using Stellamod.Core.BlackSystem;
using Stellamod.Core.Camera;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.NPCs.Town;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities
{
    public class SequenceAction
    {
        public Action startFunction;
        public Func<bool> completeCondition;
    }
    public class CutscenePlayer : ModPlayer
    {
        public override bool CanUseItem(Item item)
        {
            bool inCutscene = SequencerPlayer.IsActive();
            return base.CanUseItem(item) && !inCutscene;
        }
    }

    public class SequencerPlayer : ModSystem
    {
        private bool _debug1;
        private bool _debug2;
        private bool _debug3;
        private Sequencer _sequencer;
        private Vector2 _cameraStartPos;
        private float _cameraLerpTimer;

        private float _tintStartAlpha;
        private float _tintLerpTimer;
        public static Vector2? cameraPositionOverride;

        public static float cameraLerpTime;

        public static Color? tintColorOverride;
        public static float tintColorAlpha;
        public static float tintLerpTime;
        public override void Unload()
        {
            base.Unload();
            SetDefaults();
            _sequencer = null;
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            SetDefaults();
            _sequencer = null;
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            ManageCameraPosition();
            ManageTintColor();

            if(InputHelper.KeyUp(Keys.U) && _debug1)
            {
                DebugStartZuiWarningCutscene();
                _debug1 = false;
            }
            if (InputHelper.KeyDown(Keys.U))
            {
                _debug1 = true;
            }
            if (InputHelper.KeyUp(Keys.I) && _debug2)
            {
                DebugStartPreFightCutscene();
                _debug2 = false;
            }
            if (InputHelper.KeyDown(Keys.I))
            {
                _debug2 = true;
            }
            if (InputHelper.KeyUp(Keys.O) && _debug3)
            {
                DebugEndingCutscene();
                _debug3 = false;
            }
            if (InputHelper.KeyDown(Keys.O))
            {
                _debug3 = true;
            }
            if (_sequencer == null)
                return;


            _sequencer.Update();
            if (_sequencer.HasFinishedSequence())
            {
                _sequencer = null;
            }
        }

        private void DebugStartZuiWarningCutscene()
        {
            Cutscene zuiWarningCutscene = ModContent.GetInstance<PleaseHurryCutscene>();
            zuiWarningCutscene.Play();
        }
        private void DebugStartPreFightCutscene()
        {
            Cutscene preFightCutscene = ModContent.GetInstance<EPreFightCutscene>();
            preFightCutscene.Play();
        }
        private void DebugEndingCutscene()
        {
            Cutscene preFightCutscene = ModContent.GetInstance<EPostFightCutscene>();
            preFightCutscene.Play();
        }
        private void ManageCameraPosition()
        {
            if (cameraPositionOverride == null)
            {
                _cameraLerpTimer = 0;
                _cameraStartPos = Main.Camera.Center;
                return;
            }

            _cameraLerpTimer++;
            Vector2 overrideValue = cameraPositionOverride.Value;
            float progress = _cameraLerpTimer / cameraLerpTime;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            Vector2 interpolatedValue = Vector2.Lerp(_cameraStartPos, overrideValue, progress);
            RetargetCameraModifier.ReTargetPosition = interpolatedValue;
        }

        private void ManageTintColor()
        {
            if (tintColorOverride == null)
            {
                _tintLerpTimer = 0;
                _tintStartAlpha = FullTint.Alpha;
                return;
            }

            _tintLerpTimer++;
            Color overrideColor = tintColorOverride.Value;
            float progress = _tintLerpTimer / tintLerpTime;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            float alpha = MathHelper.Lerp(_tintStartAlpha, tintColorAlpha, progress);
            FullTint.SetColor(overrideColor, alpha);
        }

        public static void SetDefaults()
        {
            cameraPositionOverride = null;
            tintColorOverride = null;
            tintColorAlpha = 0;
            FullTint.SetColor(Color.Black, 0);
        }
        public void PlaySequence(Sequencer sequence)
        {
            _sequencer = sequence;
            _sequencer.Start();
        }
        public static bool IsActive()
        {
            SequencerPlayer player = ModContent.GetInstance<SequencerPlayer>();
            return player._sequencer != null;
        }
    }

    public class PleaseHurryCutscene : Cutscene
    {
        public override Sequencer BuildSequence()
        {
            Sequencer sequencer = new Sequencer();
            sequencer.AddDialogueAction<ZuiComeQuickDialogue>()
                .Add(() =>
                {
                    ModContent.GetInstance<EPreFightCutscene>().PrepareSequence();
                });
            return sequencer;
        }
    }

    public class EPreFightCutscene : Cutscene
    {
        private NPC Zui;
        private NPC E;

        public override void PrepareSequence()
        {
            base.PrepareSequence();
            Zui = RequireNPC<Zui>();
            E = RequireNPC<E>();

            //Now we need to set their positions
            Point spawnTile = new Point(Main.spawnTileX, Main.spawnTileY);
            Point zuiOffset = spawnTile + new Point(64, -150);
            Point eOffset = zuiOffset + new Point(32, -8);

            Zui.position = zuiOffset.ToWorldCoordinates();
            E.position = eOffset.ToWorldCoordinates();
        }

        public override Sequencer BuildSequence()
        {
            Sequencer sequencer = new Sequencer();
            sequencer
                .AddDialogueAction<ZuiWhoAreYouDialogue>()
                .AddFadeToBlack(120, 0.5f)
                .AddCameraOverride(240, E.Center)
                .AddDialogueAction<ZuiTalkingToYouDialogue>()
                .AddWait(120)
                .AddDialogueAction<EFoundYouDialogue>()
                .AddDialogueAction<ZuiGetOuttaHereDialogue>()
                .PoofNPC(Zui)
                .AddWait(120)
                .RemoveCameraOverride()
                .RemoveFadeToBlack(120)
                .Add(() =>
                {
                    E e = E.ModNPC as E;
                    e.StartFight();
                });
            return sequencer;
        }
    }

    public class EPostFightCutscene : Cutscene
    {
        private NPC E;
        public override void PrepareSequence()
        {
            base.PrepareSequence();
            E = RequireNPC<E>();
        }
        public override Sequencer BuildSequence()
        {
            Sequencer sequencer = new Sequencer();
            sequencer
                .AddCameraOverride(240, E.Center)
                .AddDialogueAction<EEndingDialogue>()
                .AddWait(120)
                .AddDialogueAction<EFearDialogue>()
                .RemoveCameraOverride();
            return sequencer;
        }
    }

    public abstract class Cutscene : ModType
    {
        protected NPC RequireNPC<T>() where T : ModNPC
        {
            Vector2 tempSpawnPoint = new Point(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
            int npcIndex = NPC.FindFirstNPC(ModContent.NPCType<T>());
            if (npcIndex == -1)
                npcIndex = NPC.NewNPC(new EntitySource_Misc("cutscene"), (int)tempSpawnPoint.X, (int)tempSpawnPoint.Y, ModContent.NPCType<T>());
            NPC zuiNPC = Main.npc[npcIndex];
            return zuiNPC;
        }

        protected sealed override void Register()
        {
            ModTypeLookup<Cutscene>.Register(this);
        }
        public override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public abstract Sequencer BuildSequence();

        public virtual void PrepareSequence()
        {

        }
        public void Play()
        {
            Sequencer sequencer = BuildSequence();
            SequencerPlayer player = ModContent.GetInstance<SequencerPlayer>();
            player.PlaySequence(sequencer);
        }
    }

    /// <summary>
    /// Handles creating a sequence of events, great for building a cutscene
    /// </summary>
    public class Sequencer
    {
        private int _index;
        private float _timer;
        private float _actionStartTime;
        private SequenceAction _action;
        private readonly List<SequenceAction> _actions;
        public Sequencer()
        {
            _actions = new List<SequenceAction>();
        }



        public Sequencer AddDialogueAction<T>() where T : BaseDialogue
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    T t = ModContent.GetInstance<T>();
                    DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
                    dialogueSystem.StartDialogueSequence(t);
                },
                completeCondition = () =>
                {
                    DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
                    return dialogueSystem.HasFinishedDialogue();
                }
            };
            _actions.Add(action);
            return this;
        }


        /// <summary>
        /// Makes the screen fade to black, this one autocompletes and doesn't wait for the fade time
        /// </summary>
        /// <param name="fadeTime"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public Sequencer AddFadeToBlack(float fadeTime, float alpha)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    SequencerPlayer.tintColorOverride = Color.Black;
                    SequencerPlayer.tintColorAlpha = alpha;
                    SequencerPlayer.tintLerpTime = fadeTime;
                },
                completeCondition = () =>
                {
                    return true;
                }
            };
            _actions.Add(action);
            return this;
        }

        public Sequencer RemoveFadeToBlack(float fadeTime)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    SequencerPlayer.tintColorOverride = Color.Black;
                    SequencerPlayer.tintColorAlpha = 0;
                    SequencerPlayer.tintLerpTime = fadeTime;
                },
                completeCondition = () =>
                {
                    return true;
                }
            };
            _actions.Add(action);
            return this;
        }

        /// <summary>
        /// Makes the camera smooth to a point
        /// </summary>
        /// <param name="lerpTime"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public Sequencer AddCameraOverride(float lerpTime, Vector2 targetPosition)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    SequencerPlayer.cameraPositionOverride = targetPosition;
                    SequencerPlayer.cameraLerpTime = lerpTime;
                },
                completeCondition = () =>
                {
                    return _timer > _actionStartTime + lerpTime;
                }
            };
            _actions.Add(action);
            return this;
        }

        public Sequencer RemoveCameraOverride()
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    SequencerPlayer.cameraPositionOverride = null;
                },
                completeCondition = () =>
                {
                    return true;
                }
            };
            _actions.Add(action);
            return this;
        }

        /// <summary>
        /// Waits the determined amount of time before going to the next action, it don't do anything!
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public Sequencer AddWait(float waitTime)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {

                },
                completeCondition = () =>
                {
                    return _timer > _actionStartTime + waitTime;
                }
            };
            _actions.Add(action);
            return this;
        }

        /// <summary>
        /// Sets an NPC to inactive and spawns dust particles around them
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public Sequencer PoofNPC(NPC npc)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {                    
                    //Spawn a bunch of dust particles or whatever
                    float numDust = 16;
                    for (int n = 0; n < numDust; n++)
                    {
                        Vector2 position = npc.position;
                        position.X += Main.rand.Next(0, npc.width);
                        position.Y += Main.rand.Next(0, npc.height);
                        
                        var smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(position, -Vector2.UnitY, Color.White, Scale: Main.rand.NextFloat(0.66f, 1.75f));
                        smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.14f);
                        smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                        smokeParticle.fadeToColor = Color.Black;
                    }
                    npc.active = false;

                },
                completeCondition = () =>
                {
                    return true;
                }
            };
            _actions.Add(action);
            return this;
        }

        public Sequencer Add(Action a)
        {
            SequenceAction action = new SequenceAction
            {
                startFunction = () =>
                {
                    a();
                },
                completeCondition = () =>
                {
                    return true;
                }
            };
            _actions.Add(action);
            return this;
        }

        public void Start()
        {
            _index = 0;
            _timer = 0;
            //Reset the defaults of the sequencer player
            SequencerPlayer.SetDefaults();
            PlayActionAndMoveNext();

  
        }

        public void Update()
        {
            if (_action == null)
                return;
            _timer++;
            if (HasFinishedAction())
            {
                PlayActionAndMoveNext();
            }
        }

        public void PlayActionAndMoveNext()
        {
            if (_index >= _actions.Count)
                return;

            _actionStartTime = _timer;
            _action = _actions[_index];
            _action.startFunction();
            _index++;
        }

        public bool HasFinishedSequence()
        {
            return _index >= _actions.Count && HasFinishedAction();
        }

        public bool HasFinishedAction()
        {
            return _action == null || _action.completeCondition();
        }
    }
}
