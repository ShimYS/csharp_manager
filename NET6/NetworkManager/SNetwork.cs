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

        #region [SERVER]

        #endregion

        #region [CLIENT]

        #endregion
    }
}