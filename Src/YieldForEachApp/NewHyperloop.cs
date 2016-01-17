using System;
using System.Collections;
using System.Collections.Generic;

namespace YieldForEachApp
{
    public abstract class NewHyperloop<T>: IHyperloop<T>, IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool loopAdded;
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

        public T Current
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

        public bool MoveNext()
        {
            while (loops != null)
            {
                do
                {
                    loopAdded = false;
                    if (loops.head.MoveNext())
                        return true;
                }
                while (loopAdded);
                DisposeLoop();
            }
            return false;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                while (loops != null)
                    DisposeLoop();
            }
        }
    }
}
