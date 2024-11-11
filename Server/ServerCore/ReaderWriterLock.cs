namespace ServerCore
{ 
    // 구현 정책
    // 1. 재귀적 락은 허용하지 않는다.
    // 2. 스핀락은 5000번 시도한 후에 Yield로 양보한다.
    
    public class ReaderWriterLock
    {
        private const int EMPTY_FLAG = 0x00000000;
        private const int WRITE_MASK = 0x7FFF0000;
        private const int READ_MASK = 0x0000FFFF;
        private const int MAX_SPIN_COUNT = 5000;

        // 32 bits : [Unused(1)] [WriteThread(15)] [ReadCount(16)]

        private int _flag = EMPTY_FLAG;

        public void WriteLock()
        {
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while (true)
            {
                for (var i = 0; i < MAX_SPIN_COUNT; i++)
                    // 아무도 사용하고 있지 않다면
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                        return;

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
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