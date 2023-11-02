namespace DatetimeManager
{
    public class Datetime
    {
        /// <summary>
        /// Datetime을 매개변수로 전달된 포맷에 맞춰 변경
        /// Default -> yyyy-MM-dd HH:mm:ss.fff
        /// example -> yyyy-MM-dd(dddd) HH:mm:ss
        ///         -> HH:mm:ss.fff
        /// </summary>
        public static string GetTimeByFormat(string format = "yyyy-MM-dd HH:mm:ss.fff")
        {
            return DateTime.Now.ToString(format);
        }

        /// <summary>
        /// Datetime을 UnixTimeMilliseconds으로 변환
        /// </summary>
        /// <param name="datetime"> Datetime </param>
        /// <returns> UnixTimeMilliseconds </returns>
        public static long ConvertDatetimeToUnixTimeMilliseconds(DateTime datetime)
        {
            return new DateTimeOffset(datetime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// UnixTimeMilliseconds을 Datetime으로 변환
        /// </summary>
        /// <param name="unixTime"> UnixTimeMilliseconds </param>
        /// <returns> Datetime </returns>
        public static DateTime ConvertUnixTimeMillisecondsToDatetime(double unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTime).ToLocalTime();
        }
    }
}