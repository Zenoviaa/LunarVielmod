using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using static Stellamod.Helpers.FunctionRepeatHelper;

namespace Stellamod.Helpers
{
    public class DelayHelper : ModSystem
    {
        private List<InvokeOnDelay> _functionsToInvoke;
        public class InvokeOnDelay
        {
            public InvokeOnDelay(float delay, Action action)
            {
                Delay = delay;
                InvokeAction = action;
            }
            public float Delay { get; set; }
            public readonly Action InvokeAction;
        }


        public override void OnModLoad()
        {
            base.OnModLoad();
            _functionsToInvoke ??= new List<InvokeOnDelay>();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            _functionsToInvoke = null;
        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            for(int i = 0; i < _functionsToInvoke.Count; i++)
            {
                InvokeOnDelay invokeOnDelay = _functionsToInvoke[i];
                invokeOnDelay.Delay--;
                if (invokeOnDelay.Delay <= 0)
                    invokeOnDelay.InvokeAction();
            }
            _functionsToInvoke.RemoveAll(x => x.Delay <= 0);
        }

        public static void Invoke(float delay, Action action)
        {
            ModContent.GetInstance<DelayHelper>()._functionsToInvoke.Add(new InvokeOnDelay(delay, action));
        }
    }
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
