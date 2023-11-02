using IniParser;
using IniParser.Model;
using System.Runtime.InteropServices;
using System.Text;

namespace InifileManager
{
    public static class Inifile
    {
        #region [Windows API]
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region [Set]
        public static void SetValue(string path, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }
        #endregion

        #region [Get]
        public static int GetValue(string Section, string Key, string def, StringBuilder retVal, int size, string path)
        {
            return GetPrivateProfileString(Section, Key, def, retVal, size, path);
        }

        public static string GetStringValue(string path, string Section, string Key, string Default)
        {
            StringBuilder temp = new StringBuilder(255);

            GetPrivateProfileString(Section, Key, Default, temp, 255, path);

            if (temp != null && temp.Length > 0)
            {
                return temp.ToString();
            }
            else
            {
                return Default;
            }
        }

        public static string GetStringValue(string path, string Section, string Key, string def, StringBuilder retVal, int size)
        {
            retVal = new StringBuilder(size);

            GetPrivateProfileString(Section, Key, def, retVal, size, path);

            if (retVal != null && retVal.Length > 0)
            {
                return retVal.ToString();
            }
            else
            {
                return def;
            }
        }

        public static int GetIntValue(string path, string Section, string Key, int Default)
        {
            StringBuilder temp = new StringBuilder(255);

            GetPrivateProfileString(Section, Key, Default.ToString(), temp, 255, path);

            if (temp != null && temp.Length > 0)
            {
                return int.Parse(temp.ToString());
            }
            else
            {
                return Default;
            }

        }

        public static int GetIntValue(string path, string Section, string Key, string def, StringBuilder retVal, int size)
        {
            retVal = new StringBuilder(size);

            GetPrivateProfileString(Section, Key, def, retVal, size, path);

            if (retVal != null && retVal.Length > 0)
            {
                return int.Parse(retVal.ToString());
            }
            else
            {
                return int.Parse(def);
            }

        }

        public static double GetDoubleValue(string path, string Section, string Key, double Default)
        {
            StringBuilder temp = new StringBuilder(255);

            GetPrivateProfileString(Section, Key, Default.ToString(), temp, 255, path);

            if (temp != null && temp.Length > 0)
            {
                return double.Parse(temp.ToString());
            }
            else
            {
                return Default;
            }
        }

        // IniFile 속 모든 Section 이름 가져오기
        public static List<string> GetAllSection(string fileName)
        {
            List<string> result = new List<string>();

            var parser = new FileIniDataParser();

            IniData data = parser.ReadFile(fileName);

            foreach (var section in data.Sections)
            {
                if (section.SectionName != "System")
                    result.Add(section.SectionName);
            }

            return result;
        }
        #endregion

        #region [Remove]
        // 섹션 전체 삭제
        public static void RemoveSection(string path, string Section)
        {
            WritePrivateProfileString(Section, null, null, path);
        }

        // 섹션, 키로 삭제
        public static void RemoveKey(string path, string Section, string key)
        {
            WritePrivateProfileString(Section, key, null, path);
        }
        #endregion
    }
}