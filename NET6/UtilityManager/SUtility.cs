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

        /// <summary>
        /// 해당 프로세서의 CPU, Memory 사용량 가져오기
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public List<(double memoryUsage, double cpuUsage)>? GetProcessUsage(string processName = "")
        {
            Process[] process = null;
            List<(double memoryUsage, double cpuUsage)> result = new List<(double memoryUsage, double cpuUsage)> ();

            if(processName == "")
                process[0] = Process.GetCurrentProcess();
            else
                process = Process.GetProcessesByName(processName);

            if(process == null)
                return null;    

            for(int i = 0; i < process.Length; i++)
            {
                result.Add((process[i].PrivateMemorySize64, (process[i].TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount)));
            }

            return result;
        }
        #endregion

        #region [Convert]
        /// <summary>
        /// 10진수를 2진수로 변환해서 반환
        /// </summary>
        /// <param name="decimalNum"></param>
        /// <returns></returns>
        public string DecimalToBinary(int decimalNum)
        {
            return Convert.ToString(decimalNum, 2);
        }

        /// <summary>
        /// 2진수를 10진수로 변환해서 반환
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public int BinaryToDecimal(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }
        #endregion

        #region [Drive]
        public struct DriveData
        {
            public string name;
            public string type;
            public string format;
            public double totalSize;   // 드라이브 총 공간
            public double totalSizeInGB;
            public double usedSpace;   // 드라이브 사용 중인 공간
            public double usedSpaceInGB;
            public double usedSpaceInPercentage;
            public double availableFreeSpace;  // 드라이브 사용 가능한 공간
            public double availableFreeSpaceInGB;
            public double availableFreeSpaceInPercentage;
        }

        /// <summary>
        /// 모든 Drive의 정보를 DriveData객체에 담아서 반환
        /// </summary>
        /// <returns></returns>
        public List<DriveData> GetDriveData()
        {
            List<DriveData> result = new List<DriveData>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach(DriveInfo drive in drives)
            {
                DriveData driveData = new DriveData();
                driveData.name = drive.Name;
                driveData.type = drive.DriveType.ToString();
                driveData.format = (drive.IsReady ? drive.DriveFormat : "미지원");
                driveData.totalSize = drive.TotalSize;
                driveData.totalSizeInGB = Math.Round((double)drive.TotalSize / 1024*1024*1024, 1);
                driveData.availableFreeSpace = drive.AvailableFreeSpace;
                driveData.availableFreeSpaceInGB = Math.Round((double)driveData.availableFreeSpace / 1024 * 1024 * 1024, 1);
                driveData.availableFreeSpaceInPercentage = Math.Round((double)(driveData.availableFreeSpace / driveData.totalSize) * 100, 1);
                driveData.usedSpace = drive.TotalSize - drive.AvailableFreeSpace;
                driveData.usedSpaceInGB = Math.Round((double)driveData.usedSpace / 1024 * 1024 * 1024, 1);
                driveData.usedSpaceInPercentage = Math.Round((double)(driveData.usedSpace / driveData.totalSize) * 100, 1);
                result.Add(driveData);
            }

            return result;
        }
        #endregion
    }
}