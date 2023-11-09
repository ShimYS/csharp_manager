namespace FileManager
{
    public class SFile
    {
        #region [File]

        #endregion

        #region [Folder] 
        /// <summary>
        /// 폴더 존재여부 검사
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ExistsDirectory(string path)
        {
            return Directory.Exists(path) ? true : false;
        }

        /// <summary>
        /// 폴더 생성
        /// </summary>
        /// <param name="path"></param>
        public void CreateFolder(string path)
        {
            if (!ExistsDirectory(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 폴더 삭제
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFolder(string path)
        {
            if (ExistsDirectory(path))
                Directory.Delete(path, true);
        }

        /// <summary>
        /// 소스폴더 내부 파일을 목적지폴더에 복사
        /// </summary>
        /// <param name="srcFolderPath"></param>
        /// <param name="destFolderPath"></param>
        public void CopyFileFromFolder(string srcFolderPath, string destFolderPath)
        {
            CreateFolder(destFolderPath);

            foreach (string file in Directory.GetFiles(srcFolderPath, "*.*", SearchOption.AllDirectories))
            {
                string destinationFilePath = file.Replace(srcFolderPath, destFolderPath);

                File.Copy(file, destinationFilePath);
            }
        }
        #endregion
    }
}