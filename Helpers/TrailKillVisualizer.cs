using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class TrailKillSystem : ModSystem
    {
        private List<TrailKillVisualizer> _trails;
        public override void Load()
        {
            base.Load();
            _trails = new List<TrailKillVisualizer>();
            On_Main.DrawDust += DrawTrails;
        }

        public override void Unload()
        {
            base.Unload();
            On_Main.DrawDust -= DrawTrails;
        }
        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            foreach (var trail in _trails)
            {
                trail.Update();
            }
            _trails.RemoveAll(x => x.kill);
        }
        public void New(Vector2[] oldPos, IDrawTrail trailDrawFunc)
        {
            _trails.Add(new TrailKillVisualizer(oldPos, trailDrawFunc));
        }
        private void DrawTrails(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            foreach (var trail in _trails)
            {
                trail.Draw();
            }
        }
    }
    public class TrailKillVisualizer
    {

        public int timer;
        public int index;
        public bool kill;
        public Vector2[] oldPos;
        public IDrawTrail trailDrawFunc;
        public TrailKillVisualizer(Vector2[] oldPos, IDrawTrail trailDrawFunc)
        {
            this.index = oldPos.Length - 1;
            this.oldPos = oldPos;
            this.trailDrawFunc = trailDrawFunc;
        }

        public void Update()
        {
            timer++;
            if (timer >= 1)
            {
                timer = 0;
                oldPos[index] = new Vector2(9999);
                index--;
            }
            if (index < 0)
            {
                kill = true;
            }
        }
        public void Draw()
        {
            trailDrawFunc.DrawTrail(oldPos);
        }
    }
}
