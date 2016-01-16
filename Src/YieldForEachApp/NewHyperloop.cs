using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{
    public abstract class NewHyperloop<T>: IHyperloop<T>, IEnumerator<T>, IEnumerator, IDisposable
    {
        //private bool loopAdded;
        protected Sequence<IEnumerator<T>> loops;

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
