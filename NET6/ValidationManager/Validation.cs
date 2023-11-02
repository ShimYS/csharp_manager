using System.Text.RegularExpressions;

namespace ValidationManager
{
    public class Validation
    {
        #region [String]
        // 숫자인지 확인
        public static bool IsNumeric(string input)
        {
            return double.TryParse(input, out _);
        }

        // 정수인지 확인
        public static bool IsInteger(string input)
        {
            return int.TryParse(input, out _);
        }

        // 문자열이 비어 있는지 확인
        public static bool IsNullOrEmpty(string input)
        {
            return string.IsNullOrEmpty(input);
        }

        // 최소/최대 길이 검사
        public static bool IsWithinLength(string input, int minLength, int maxLength)
        {
            if (input == null)
                return false;
            return input.Length >= minLength && input.Length <= maxLength;
        }

        // 문자열이 알파벳 문자만 포함하는지 확인
        public static bool ContainsOnlyAlphabets(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(char.IsLetter);
        }
        #endregion

        #region [Network]
        // IP 주소 유효성 검사
        public static bool IsValidIPAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }

        // MAC 주소 유효성 검사
        public static bool IsValidMACAddress(string macAddress)
        {
            if (string.IsNullOrEmpty(macAddress))
                return false;
            return System.Text.RegularExpressions.Regex.IsMatch(macAddress, "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        }

        // 파일 경로 유효성 검사
        public static bool IsValidFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return System.IO.Path.IsPathRooted(path);
        }
        #endregion

        #region [Web]
        // 이메일 주소 유효성 검사
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            return Regex.IsMatch(email, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }

        // URL 유효성 검사
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
        #endregion

        #region [Etc]
        // 주민등록번호 유효성 검사 (대한민국 기준)
        public static bool IsValidKoreanResidentRegistrationNumber(string rrn)
        {
            if (string.IsNullOrEmpty(rrn) || rrn.Length != 13)
                return false;
            // 여기에서 유효성 검사 규칙을 적용할 수 있습니다.
            // 예를 들어, 생년월일, 성별 등을 확인할 수 있습니다.
            // 실제 유효성 검사 규칙은 국가별로 다를 수 있습니다.
            return true;
        }

        // 비밀번호 복잡성 유효성 검사
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;
            // 여기에서 복잡성 조건을 적용할 수 있습니다.
            // 예를 들어, 최소 길이, 대/소문자, 특수 문자 등을 확인할 수 있습니다.
            return password.Length >= 8 && System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$");
        }

        // 정규식 패턴과 일치하는지 확인
        public static bool MatchesPattern(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }
        #endregion

        #region [Date]
        // 날짜 형식 유효성 검사
        public static bool IsValidDate(string date, string format = "yyyy-MM-dd")
        {
            return DateTime.TryParseExact(date, format, null, System.Globalization.DateTimeStyles.None, out _);
        }
        #endregion
    }
}