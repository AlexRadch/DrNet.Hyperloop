using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{
    public interface IHyperloop</*out*/ T>
    {
        void AddLoop(IEnumerator<T> loop);
        void AddTail(IEnumerator<T> loop);
    }

    public interface ILoopProvider</*out*/ T>
    {
        IEnumerator<T> GetLoop(IHyperloop<T> hyperloop);
    }

    public sealed class Hyperloop<T>: IHyperloop<T>, ILoopProvider<T>, 
        IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
    {
        //private bool loopAdded;
        private Sequence<IEnumerator<T>> loops;

        public Hyperloop(IEnumerable<T> loop)
        {
            ((IHyperloop<T>)this).AddLoop(loop.GetEnumerator());
        }

        public Hyperloop(IEnumerator<T> loop)
        {
            ((IHyperloop<T>)this).AddLoop(loop);
        }

        void IHyperloop<T>.AddLoop(IEnumerator<T> loop)
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

        void IHyperloop<T>.AddTail(IEnumerator<T> loop)
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

        IEnumerator<T> ILoopProvider<T>.GetLoop(IHyperloop<T> hyperloop) // can get control from other accessibility level
        {
            if (loops != null && loops.tail == null) // should be true always
            {
                var loop = loops.head;
                loops = null;
                return loop;
            }
            else
                return this; // should not get control any time
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
                do
                {
                    oldLoops = loops;
                    if (loops.head.MoveNext())
                        return true;
                }
                while (oldLoops != loops);
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

    internal class Sequence<T>
    {
        public T head;
        public Sequence<T> tail;

        public Sequence(T head)
        {
            this.head = head;
            tail = null;
        }

        public Sequence(T head, Sequence<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }
    }
}
