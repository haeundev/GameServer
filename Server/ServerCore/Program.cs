namespace ServerCore;

internal class Program
{
    private static int number = 0;

    private static void Thread1()
    {
        for (var i = 0; i < 1000000; i++)
        {
            number++;
        }
    }

    private static void Thread2()
    {
        for (var i = 0; i < 1000000; i++)
        {
            number--;
        }
    }

    private static void Main(string[] args)
    {
        var t1 = new Task(Thread1);
        var t2 = new Task(Thread2);

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(number);
    }
}