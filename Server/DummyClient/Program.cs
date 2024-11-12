using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
            
            // 클라이언트 소켓 생성
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                // 서버에 연결
                socket.Connect(endPoint);
                Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}");

                // 서버에 데이터 전송
                byte[] sendBuff = Encoding.UTF8.GetBytes("Hello, Server!");
                int sendBytes = socket.Send(sendBuff);

                // 서버로부터 데이터 수신
                byte[] recvBuff = new byte[1024];
                int recvBytes = socket.Receive(recvBuff);
                var recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Server] {recvData}");

                // 서버 소켓 닫기
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}