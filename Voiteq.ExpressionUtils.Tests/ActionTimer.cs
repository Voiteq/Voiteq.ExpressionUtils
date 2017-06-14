using System;
using System.Diagnostics;

namespace Voiteq.ExpressionUtils.Tests
{
    public static class ActionTimeExtensions
    {
        public static ActionTimer RunRepeatedly(this Action action, long repetitions) => new ActionTimer(action, repetitions);
    }
    
    public class ActionTimer
    {
        private readonly Stopwatch _stopWatch = new Stopwatch();

        internal ActionTimer(Action action, long repetitions)
        {
            _stopWatch.Start();
            for (var i = 0; i < repetitions; i++) action();
            _stopWatch.Stop();
        }

        public long TimeTaken => _stopWatch.ElapsedMilliseconds;
    }
}