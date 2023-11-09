using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetworkManager
{
    public class SNetwork
    {
        #region [IP]
        /// <summary>
        /// 로컬 IP주소 가져오기
        /// </summary>
        /// <returns></returns>
        public string GetLocalIP()
        {
            string localIP = string.Empty;
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        /// <summary>
        /// 해당 IP의 장치이름 가져오기
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string GetIPDeviceName(string ip)
        {
            string result = "";

            try
            {
                result = Dns.GetHostEntry(IPAddress.Parse(ip)).HostName.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 해당 IP에 대한 Ping 테스트
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>
        /// 성공 => "주소, 응답시간, 데이터의 유효 기간, 손실데이터 유무, 버퍼사이즈"
        /// 실패 => ""
        /// </returns>
        public string PingTest(string ip)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(IPAddress.Parse(ip), timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                string str = string.Empty;
                str += "Address: " + reply.Address.ToString() + "\n";   // 주소
                str += "RoundTrip time: " + reply.RoundtripTime + "ms\n";   // 응답시간
                str += "Time to live: " + reply.Options.Ttl + "\n";   // TTL : 데이터의 유효 기간
                str += "Don't fragment: " + reply.Options.DontFragment + "\n";   // 손실데이터 유무
                str += "Buffer size: " + reply.Buffer.Length + "\n\n";   // 버퍼사이즈

                return str;
            }
            else
            {
                return "";
            }
        }
        #endregion
    }

    public class SClient
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private TcpClient? _server;
        private Action<string, byte[]> _processDataAction;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="ipAddress">IP 주소</param>
        /// <param name="port">Port 번호</param>
        /// <param name="processDataAction">서버로 부터 수신한 데이터를 처리하는 메소드</param>
        public SClient(IPAddress ipAddress, int port, Action<string, byte[]> processDataAction)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            _processDataAction = processDataAction;
        }

        /// <summary>
        /// 서버와 연결 
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            try
            {
                _server = new TcpClient();
                await _server.ConnectAsync(ipAddress, port);
                StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConnectAsync()] {ex.Message}");
                _processDataAction?.Invoke($"{ipAddress}, {port} 연결 실패", null);
            }
        }

        private async Task StartListening()
        {
            _processDataAction?.Invoke($"{ipAddress}, {port} 연결 성공", null);

            var buffer = new byte[1024];


            while (true)
            {

                var numBytesRead = await _server.GetStream().ReadAsync(buffer, 0, buffer.Length);

                if (numBytesRead == 0)
                {
                    break;
                }

                // 데이터처리
                _processDataAction?.Invoke(numBytesRead.ToString(), buffer);
            }

            _server.Close();
        }

        /// <summary>
        /// 연결된 서베에 데이터 전송
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(string message)
        {
            try
            {
                Stream stream = _server.GetStream();
                var buffer = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"[SendAsync()] {ex.Message}");
                Disconnect();
            }
        }

        /// <summary>
        /// 서버와의 연결 끊기
        /// </summary>
        public void Disconnect()
        {
            _server.Close();
            _processDataAction?.Invoke($"{ipAddress}, {port} 연결 끊김", null);
        }

        /// <summary>
        /// 서버가 클라이언트와 연결되어 있는지 체크
        /// </summary>
        /// <returns>연결된 상태 - true / 연결이 끊긴 상태 - false</returns>
        public bool Server_IsConnected()
        {
            if (_server == null)
            {
                return false;
            }
            else if (!_server.Connected)
            {
                return false;
            }
            return true;
        }
    }
}