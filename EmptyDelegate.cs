using System;

namespace _15puzzleWPF
{
    internal class EmptyDelegate
    {
        private Func<object> p;

        public EmptyDelegate(Func<object> p)
        {
            this.p = p;
        }
    }
}