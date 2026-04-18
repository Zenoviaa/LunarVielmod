using Stellamod.Helpers;
using Stellamod.NPCs.Town;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;

namespace Stellamod.Core.Camera
{
    public class CameraTargetSystem : ModSystem
    {
        public List<Vector2> TargetPositions;
        public Vector2 reTargetPosition;
        public float reTargetLerp;
        public float reTargetTimer;
        public float easingTime => 60;
        public override void Load()
        {
            base.Load();
            TargetPositions = new List<Vector2>();
        }
        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            TargetPositions.Clear();
        }
        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            reTargetLerp = EasingFunction.InOutSine(reTargetTimer / easingTime);
            if (TargetPositions.Count <= 0)
            {
                if(reTargetTimer > 0)
                {
                    reTargetTimer--;
                }
                return;
            }

            reTargetTimer++;
            if (reTargetTimer >= easingTime)
                reTargetTimer = easingTime;

            Vector2 total = Vector2.Zero;
            for (int i = 0; i < TargetPositions.Count; i++)
            {
                total += TargetPositions[i];
            }
            Vector2 newTargetPosition = total / TargetPositions.Count;
            reTargetPosition = Vector2.Lerp(reTargetPosition, newTargetPosition, 0.08f);
        }
        public static void AddTarget(Vector2 position)
        {
            ModContent.GetInstance<CameraTargetSystem>().TargetPositions.Add(position);
        }
    }
    public class RetargetCameraModifier : ICameraModifier
    {
        private Vector2 _cameraOffset;
        public string UniqueIdentity => "retarget";
        public bool Finished => !CameraSystem.IsLoaded;

        private CameraTargetSystem TargetSystem => ModContent.GetInstance<CameraTargetSystem>();
        public void Update(ref CameraInfo cameraPosition)
        {
            Vector2 targetPosition = (TargetSystem.reTargetPosition - cameraPosition.CameraPosition);
            Vector2 screenBounds = new Vector2(Main.screenWidth, Main.screenHeight);
            screenBounds *= 0.5f;
            targetPosition -= screenBounds;
            _cameraOffset = Vector2.Lerp(Vector2.Zero, targetPosition, TargetSystem.reTargetLerp);
            cameraPosition.CameraPosition = cameraPosition.CameraPosition + _cameraOffset;
        }
    }
}
