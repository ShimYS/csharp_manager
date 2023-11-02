using System.IO.Ports;

namespace SerialManager
{
    public class Serial
    {
        public string serialName;
        private SerialPort serialPort;
        private Action<string> processReceiveData;

        public Serial(string name, int baudrate, int databits, Parity parity, StopBits stopbits, bool handshake, Action<string> processReceiveData)
        {
            serialName = name;
            serialPort = new SerialPort(name, baudrate, parity, databits, stopbits);
            serialPort.DataReceived += SerialPort_DataReceived;
            this.processReceiveData = processReceiveData;

            // RS485통신용
            if (handshake)
            {
                serialPort.Handshake = Handshake.RequestToSend;
                serialPort.RtsEnable = true;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();

            processReceiveData?.Invoke(data);
        }


        /// <summary>
        /// 시리얼포트 오픈 상태를 반환한다.
        /// </summary>
        /// <returns>
        /// Open - true 반환
        /// Close - false 반환
        /// </returns>
        public bool Status()
        {
            return serialPort.IsOpen == true ? true : false;
        }

        /// <summary>
        /// 시리얼포트 오픈
        /// </summary>
        public void OpenPort()
        {
            if (serialPort.IsOpen == false)
                serialPort.Open();
        }

        /// <summary>
        /// 시리얼포트 클로즈
        /// </summary>
        public void ClosePort()
        {
            if (serialPort.IsOpen == true)
            {
                serialPort.Close();
            }
        }

        public void DisposePort()
        {
            serialPort.Dispose();
        }

        /// <summary>
        /// 데이터 송신
        /// </summary>
        /// <param name="sendData"></param>
        public void SendData(string sendData)
        {
            if (serialPort.IsOpen == true)
            {
                serialPort.WriteLine(sendData);
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        /// <summary>
        /// 데이터 수신
        /// </summary>
        /// <param name="receiveData"></param>
        /*public void ReceiveData(out string receiveData)
        {
            if(serialPort.IsOpen == true)
            {
                receiveData = serialPort.ReadLine();
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
                receiveData = null;
            }
        }*/
    }
}