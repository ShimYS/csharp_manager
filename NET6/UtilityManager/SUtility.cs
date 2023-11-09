using System.Diagnostics;

namespace UtilityManager
{
    public class SUtility
    {
        #region [Process]
        /// <summary>
        /// 프로세스 이름으로 해당 프로세스 유무 확인
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public bool CheckProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
                return true;

            return false;
        }

        /// <summary>
        /// 프로세스 이름으로 해당 프로세스 강제 종료
        /// </summary>
        /// <param name="processName"></param>
        public void KillProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    if (process.ProcessName == processName)
                        process.Kill();
                }
            }
        }
        #endregion

        #region [Convert]
        /// <summary>
        /// 10진수를 2진수로 변환해서 반환
        /// </summary>
        /// <param name="decimalNum"></param>
        /// <returns></returns>
        public static string DecimalToBinary(int decimalNum)
        {
            return Convert.ToString(decimalNum, 2);
        }

        /// <summary>
        /// 2진수를 10진수로 변환해서 반환
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static int BinaryToDecimal(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }
        #endregion
    }
}