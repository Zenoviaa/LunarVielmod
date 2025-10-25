using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class FunctionRepeatHelper : ModSystem
    {
        private List<Repeater> _repeaters;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _repeaters = new List<Repeater>();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            _repeaters = null;
        }

        public override void PostUpdateItems()
        {
            base.PostUpdateItems();
            if (_repeaters.Count <= 0)
                return;
            _repeaters.RemoveAll(x => x.repeats <= 0);
            foreach (var repeat in _repeaters)
            {
                repeat.timer++;
                if (repeat.timer >= repeat.rate)
                {
                    repeat.repeats--;
                    repeat.timer = 0;
                    repeat.function();
                }
            }
        }
        public class Repeater
        {
            public float timer;
            public Action function;
            public int repeats;
            public float rate;
        }
        public static void Repeat(Action function, int repeats, float rate)
        {
            Repeater repeater = new Repeater
            {
                timer = rate,
                function = function,
                repeats = repeats,
                rate = rate
            };
            ModContent.GetInstance<FunctionRepeatHelper>()._repeaters.Add(repeater);
        }
    }
}
