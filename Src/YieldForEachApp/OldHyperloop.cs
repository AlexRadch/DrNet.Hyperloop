using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{
    public interface IHyperloopProvider</*out*/ T>
    {
        IHyperloop<T> GetHyperloop();
    }

    public sealed class OldHyperloop<T> : IHyperloop<T>, ILoopProvider<T>, IHyperloopProvider<T>,
        IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
    {
        //private bool loopAdded;
        private Sequence<IEnumerator<T>> loops;
        private IHyperloop<T> hyperloop;

        public void AddLoop(IEnumerator<T> loop)
        {
            try
            {
                loops = new Sequence<IEnumerator<T>>(loop, loops);
            }
            catch
            {
                loop.Dispose();
                throw;
            }
            //loopAdded = true;
        }

        public void AddTail(IEnumerator<T> loop)
        {
            try
            {
                DisposeLoop();
                loops = new Sequence<IEnumerator<T>>(loop, loops);
            }
            catch
            {
                loop.Dispose();
                throw;
            }
            //loopAdded = true;
        }

        private void DisposeLoop()
        {
            if (loops == null)
                return;
            var loop = loops.head;
            loops = loops.tail;
            loop.Dispose();
        }

        IEnumerator<T> ILoopProvider<T>.GetLoop(IHyperloop<T> hyperloop)
        {
            if (loops != null && loops.tail == null) // should be true always
            {
                this.hyperloop = hyperloop;
                var loop = loops.head;
                loops = null;
                return loop;
            }
            else
                return this; // should not get control any time
        }

        IHyperloop<T> IHyperloopProvider<T>.GetHyperloop()
        {
            if (hyperloop == null)
                return this;
            return hyperloop;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        T IEnumerator<T>.Current
        {
            get
            {
                return loops.head.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return loops.head.Current;
            }
        }

        bool IEnumerator.MoveNext()
        {
            while (loops != null)
            {
                Sequence<IEnumerator<T>> oldLoops;
                bool moveNext;
                do
                {
                    oldLoops = loops;
                    moveNext = loops.head.MoveNext();
                }
                while (oldLoops != loops);
                if (moveNext)
                    return true;
                DisposeLoop();
            }
            return false;
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        void IDisposable.Dispose()
        {
            while (loops != null)
                DisposeLoop();
        }
    }

}
