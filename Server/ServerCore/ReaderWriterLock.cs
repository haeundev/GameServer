namespace ServerCore
{ 
    // 구현 정책
    // 1. 재귀적 락을 허용한다.
    //   - 동일한 스레드가 여러 번 WriteLock을 할 수 있다.
    //   - 동일한 스레드가 WriteLock을 획득한 상태에서 ReadLock을 할 수 있다.
    //   - 동일한 스레드가 ReadLock을 획득한 상태에서 WriteLock을 할 수는 없다.
    // 2. 스핀락은 5000번 시도한 후에 Yield로 양보한다.
    
    public class ReaderWriterLock
    {
        private const int EMPTY_FLAG = 0x00000000;
        private const int WRITE_MASK = 0x7FFF0000;
        private const int READ_MASK = 0x0000FFFF;
        private const int MAX_SPIN_COUNT = 5000;

        // 32 bits : [Unused(1)] [WriteThread(15)] [ReadCount(16)]

        private int _flag = EMPTY_FLAG;
        private int _writeCount = 0;

        public void WriteLock()
        {
            // 동일한 스레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (lockThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                _writeCount++;
                return;
            }
            
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {
                for (var i = 0; i < MAX_SPIN_COUNT; i++)
                    // 아무도 WriteLock 또는 ReadLock을 사용하고 있지 않다면, WriteLock을 획득한다.
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일한 스레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (lockThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }
            
            while (true)
            {
                // 아무도 WriteLock을 사용하고 있지 않다면, ReadLock을 1 증가시킨다.
                for (var i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK); // write lock을 다 무시하고 read lock만 확인
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }
            }
        }
        
        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}