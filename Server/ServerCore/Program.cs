using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    internal class Program
    {
        private static readonly Listener _listener = new();

        private static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 클라이언트로부터 받은 데이터를 저장할 버퍼
                var recvBuff = new byte[1024];
                var recvBytes = clientSocket.Receive(recvBuff); // 클라이언트로부터 받은 데이터의 바이트 수
                var recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes); // 바이트 배열을 문자열로 변환
                Console.WriteLine($"[From Client] {recvData}");

                // 클라이언트에게 답장을 보낼 데이터
                var sendBuff = Encoding.UTF8.GetBytes("Welcome to Server!");
                clientSocket.Send(sendBuff); // 클라이언트에게 데이터 전송

                // 클라이언트 소켓 닫기
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void Main(string[] args)
        {
            // DNS (Domain Name System)
            var host = Dns.GetHostName(); // 현재 컴퓨터의 호스트 이름을 가져옴
            var ipHost = Dns.GetHostEntry(host); // 호스트 이름에 해당하는 IP 주소들을 가져옴
            var ipAddr = ipHost.AddressList[0]; // IP 주소들 중 첫 번째 주소를 선택
            var endPoint = new IPEndPoint(ipAddr, 7777); // IP 주소와 포트 번호를 묶어서 IPEndPoint 객체 생성

            _listener.Init(endPoint, OnAcceptHandler);

            while (true)
            {
            }
        }
    }
}