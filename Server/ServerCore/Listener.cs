using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public class Listener
{
    Socket _listenSocket;
    Action<Socket> _onAcceptHandler;
    
    public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _onAcceptHandler += onAcceptHandler;
        _listenSocket.Bind(endPoint); // 소켓에 IP 주소와 포트 번호를 바인딩
        _listenSocket.Listen(10); // 최대 대기수(backlog) 10: 동시에 10개의 클라이언트만 접속 가능
        
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptComplete);
        RegisterAccept(args); // 클라이언트 접속 처리를 시작
    }
    
    void RegisterAccept(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;
        
        bool isPending = _listenSocket.AcceptAsync(args);
        if (isPending == false) // 클라이언트 접속 처리가 즉시 완료됨
        {
            OnAcceptComplete(null, args);
        }
    }

    void OnAcceptComplete(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            _onAcceptHandler.Invoke(args.AcceptSocket);
        }
        else
        {
            Console.WriteLine(args.SocketError.ToString());
        }
        
        RegisterAccept(args);
    }
}