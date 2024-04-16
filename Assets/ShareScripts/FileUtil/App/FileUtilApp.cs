using System.Text;
using UnityEngine;

#if GAME_PLATFORM_EDITOR || GAME_PLATFORM_ANDROID || GAME_PLATFORM_IOS
//编辑器和app的文件操作
namespace SharePublic
{
    public class FileUtilApp : FileUtilBase
    {
        //同步读取文本文件
        public override string LoadFileTextSync(string path)
        {
            string loadStr = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                return loadStr;
            }
            try
            {
                if (System.IO.File.Exists(path))
                {
                    loadStr = System.IO.File.ReadAllText(path, Encoding.UTF8);
                }
                return loadStr;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"读取文件失败 {path} 内容 {loadStr}");
                return loadStr;
            }
        }

        //同步读取数据文件
        public override byte[] LoadFileBytesSync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            try
            {
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllBytes(path);
                }
                return null;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"读取Bytes文件失败 {path}");
                return null;
            }
        }

        //保文本文件
        public override void SaveFileText(string path, string saveText)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                System.IO.File.WriteAllText(path, saveText, Encoding.UTF8);
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"保存文件失败 {path} 内容 {saveText}");
            }
        }

        //同步保存文本文件
        public override bool SaveFileTextSync(string path, string saveText)
        {
            SaveFileText(path, saveText);
            return true;
        }

        //保存数据文件
        public override void SaveFileBytes(string path, byte[] saveBytes)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                System.IO.File.WriteAllBytes(path, saveBytes);
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"保存Bytes文件失败 {path}");
            }
        }

        //同步保存数据文件
        public override bool SaveFileBytesSync(string path, byte[] saveBytes)
        {
            SaveFileBytes(path, saveBytes);
            return true;
        }

        //追加数据内容
        public override void AppendFile(string path, byte[] saveBytes)
        {
        }

        //追加文本内容
        public override void AppendFile(string path, string saveText)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                System.IO.File.AppendAllText(path, saveText);
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"{path} 追加文件内容失败 {saveText}");
            }
        }

        //同步追加数据内容
        public override bool AppendFileSync(string path, byte[] saveBytes)
        {
            return false;
        }

        //同步追加文本内容
        public override bool AppendFileSync(string path, string saveText)
        {
            AppendFile(path, saveText);
            return true;
        }

        //删除文件失败
        public override bool DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return true;

            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"删除文件失败 {path}");
                return false;
            }
        }

        //创建目录失败
        public override bool Mkdir(string dirPath)
        {
            try
            {
                System.IO.Directory.CreateDirectory(dirPath);
                return true;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"创建目录失败 {dirPath}");
                return false;
            }
        }

        //删除目录失败
        public override bool Rmdir(string dirPath, bool recursive = true)
        {
            try
            {
                System.IO.Directory.Delete(dirPath, recursive);
                return true;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError($"删除目录失败 {dirPath}");
                return false;
            }
        }

        //文件是否存在
        public override bool IsExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                return System.IO.File.Exists(path);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        //获得持久化目录
        public override string GetPersitentPath()
        {

#if UNITY_EDITOR
            return Application.dataPath;
#else
            return Application.persistentDataPath;
#endif
        }
    }
}
#endif
