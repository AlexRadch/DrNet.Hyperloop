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

    public interface IOldHyperloop</*out*/ T>: IHyperloop<T>, IHyperloopProvider<T>
    { }

    public interface IHyperloopProvider</*out*/ T>
    {
        IOldHyperloop<T> GetHyperloop();
    }

    public sealed class Hyperloop<T> : IHyperloop<T>, ILoopProvider<T>, IHyperloopProvider<T>, IOldHyperloop<T>,
        IEnumerable<T>, IEnumerable, IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool loopAdded;
        private Sequence<IEnumerator<T>> loops;
        private IOldHyperloop<T> hyperloop;

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
            loopAdded = true;
        }

        public void AddTail(IEnumerator<T> loop)
        {
            if (loops == null)
                AddLoop(loop);
            else
            {
                var oldLoop = loops.head;
                loops.head = loop;
                loop = null;
                oldLoop.Dispose();
                loopAdded = true;
            }
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
                this.hyperloop = hyperloop as IOldHyperloop<T>;
                if (this.hyperloop != null) // should be true always
                {
                    var loop = loops.head;
                    loops = null;
                    return loop;
                }
            }
            return this; // should not get control any time
        }

        IOldHyperloop<T> IHyperloopProvider<T>.GetHyperloop()
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
                bool moveNext;
                do
                {
                    loopAdded = false;
                    moveNext = loops.head.MoveNext();
                }
                while (loopAdded);
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

        public void Dispose()
        {
            while (loops != null)
            {
                var loop = loops.head;
                loops = loops.tail;
                loop.Dispose();
            }
        }
    }

    public class Sequence<T>
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
