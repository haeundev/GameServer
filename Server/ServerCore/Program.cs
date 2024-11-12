using System;
using System.Threading;

class GameServer
{
    // 각 스레드가 클라이언트 연결 상태를 저장하기 위한 TLS 변수
    private static ThreadLocal<ConnectionState> _connectionState = new ThreadLocal<ConnectionState>(() => new ConnectionState());

    static void Main(string[] args)
    {
        // 예를 들어 클라이언트 처리용 스레드 시작
        Thread clientHandler1 = new Thread(HandleClient);
        Thread clientHandler2 = new Thread(HandleClient);
        Thread clientHandler3 = new Thread(HandleClient);

        clientHandler1.Start();
        clientHandler2.Start();
        clientHandler3.Start();

        clientHandler1.Join();
        clientHandler2.Join();
        clientHandler3.Join();
    }

    static void HandleClient()
    {
        // 각 스레드가 고유한 연결 상태를 설정
        _connectionState.Value.ClientId = Thread.CurrentThread.ManagedThreadId;
        _connectionState.Value.Status = "Connected";

        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Client ID: {_connectionState.Value.ClientId}, Status: {_connectionState.Value.Status}");

        // 클라이언트 처리 로직을 수행
        // 예: 게임 패킷 처리, 상태 업데이트 등

        // 연결 상태를 업데이트
        _connectionState.Value.Status = "Disconnected";
        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Client ID: {_connectionState.Value.ClientId}, Status: {_connectionState.Value.Status}");
    }
}

// 클라이언트의 연결 상태를 나타내는 클래스
class ConnectionState
{
    public int ClientId { get; set; }
    public string Status { get; set; }
}