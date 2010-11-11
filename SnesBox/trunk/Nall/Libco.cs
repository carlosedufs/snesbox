﻿using System.Threading;

namespace Nall
{
    public static class Libco
    {
        private static Thread _active;

        public static Thread Active()
        {
            if (ReferenceEquals(_active, null))
            {
                _active = Thread.CurrentThread;
            }
            return _active;
        }

        public static Thread Create(string name, int size, ThreadStart entrypoint)
        {
            if (ReferenceEquals(_active, null))
            {
                _active = Thread.CurrentThread;
            }

            size += 256; /* allocate additional space for storage */
            size &= ~15; /* align stack to 16-byte boundary */
            return new Thread(entrypoint, size) { Name = name };
        }

        public static void Delete(Thread handle)
        {
            handle.Abort();
            handle = null;
        }

        public static void Switch(Thread handle)
        {
            var previous = _active;
            _active = handle;
            if (_active.ThreadState == ThreadState.Unstarted)
            {
                _active.Start();
            }
            else
            {
                while (_active.ThreadState != ThreadState.Suspended) { }
                _active.Resume();
            }
            previous.Suspend();
        }
    }
}