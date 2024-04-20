#if GAME_PLATFORM_WEIXIN
using System;
using WeChatWASM;
//微信的文件操作
namespace SharePublic
{
    public class FileUtilWx : FileUtilBase
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
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                string errorCode = fileSystem.AccessSync(path);
                if (!errorCode.ToLower().Contains("ok"))
                {
                    if (MainUtils.IsDebugOpen())
                    {
                        UnityEngine.Debug.Log($"{path} 不存在");
                    }
                    return loadStr;
                }
                loadStr = fileSystem.ReadFileSync(path, "utf8");
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path} 读取的内容{loadStr}");
                }
                return loadStr;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 读取失败 {loadStr}");
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
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                fileSystem = WX.GetFileSystemManager();
                string errorCode = fileSystem.AccessSync(path);
                if (!errorCode.ToLower().Contains("ok"))
                {
                    if (MainUtils.IsDebugOpen())
                    {
                        UnityEngine.Debug.Log($"本地Bytes {path} 不存在");
                    }
                    return null;
                }
                byte[] fileBytes = fileSystem.ReadFileSync(path);
                return fileBytes;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path} 读取失败");
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
            WXFileSystemManager fileSystem = WX.GetFileSystemManager();
            try
            {
                fileSystem = WX.GetFileSystemManager();
                string access = fileSystem.AccessSync(path);
                if (access.ToLower().Contains("ok"))
                {
                    fileSystem.UnlinkSync(path);
                }
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 保存前，数据清除失败");
            }
            try
            {
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path} 保存文件的内容 {saveText}");
                }
                fileSystem.WriteFile(new WriteFileStringParam()
                {
                    filePath = path,
                    encoding = "utf8",
                    data = saveText,
                });

            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 保存失败 {saveText}");
            }
        }

        //同步保存文本文件
        public override bool SaveFileTextSync(string path, string saveText)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            WXFileSystemManager fileSystem = WX.GetFileSystemManager();
            try
            {
                fileSystem = WX.GetFileSystemManager();
                string access = fileSystem.AccessSync(path);
                if (access.ToLower().Contains("ok"))
                {
                    fileSystem.UnlinkSync(path);
                }
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 保存前，数据清除失败");
            }
            try
            {
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path} 保存文件的内容 {saveText}");
                }
                fileSystem.WriteFileSync(path, saveText, "utf8");
                return true;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 保存失败 {saveText}");
                return false;
            }
        }

        //保存数据文件
        public override void SaveFileBytes(string path, byte[] saveBytes)
        {
            WXFileSystemManager fileSystem = WX.GetFileSystemManager();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                fileSystem = WX.GetFileSystemManager();
                string access = fileSystem.AccessSync(path);
                if (access.ToLower().Contains("ok"))
                {
                    fileSystem.UnlinkSync(path);
                }
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path} 保存前，数据清除失败");
            }
            try
            {
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                fileSystem.WriteFile(new WriteFileParam()
                {
                    filePath = path,
                    encoding = "utf8",
                    data = saveBytes,
                });
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
            }
        }

        //同步保存数据文件
        public override bool SaveFileBytesSync(string path, byte[] saveBytes)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            WXFileSystemManager fileSystem = WX.GetFileSystemManager();
            try
            {
                fileSystem = WX.GetFileSystemManager();
                string access = fileSystem.AccessSync(path);
                if (access.ToLower().Contains("ok"))
                {
                    fileSystem.UnlinkSync(path);
                }
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path} 保存前，数据清除失败");
            }
            try
            {
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                fileSystem.WriteFileSync(path, saveBytes, "utf8");
                return true;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
                return false;
            }
        }

        //追加数据内容
        public override void AppendFile(string path, byte[] saveBytes)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                WriteFileParam writeFileParam = new WriteFileParam();
                writeFileParam.filePath = path;
                writeFileParam.data = saveBytes;
                writeFileParam.encoding = "utf8";
                fileSystem.AppendFile(writeFileParam);
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
            }
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
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                WriteFileStringParam writeFileParam = new WriteFileStringParam();
                writeFileParam.filePath = path;
                writeFileParam.data = saveText;
                writeFileParam.encoding = "utf8";
                fileSystem.AppendFile(writeFileParam);
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
            }
        }

        //同步追加数据内容
        public override bool AppendFileSync(string path, byte[] saveBytes)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                fileSystem.AppendFileSync(path, saveBytes, "utf8");
                return true;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
                return false;
            }
        }

        //同步追加文本内容
        public override bool AppendFileSync(string path, string saveText)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                if (MainUtils.IsDebugOpen())
                {
                    UnityEngine.Debug.Log($"{path}");
                }
                fileSystem.AppendFileSync(path, saveText, "utf8");
                return true;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地Bytes文件 {path}");
                return false;
            }
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
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                fileSystem = WX.GetFileSystemManager();
                string access = fileSystem.AccessSync(path);
                if (access.ToLower().Contains("ok"))
                {
                    fileSystem.UnlinkSync(path);
                }
                return true;
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError($"本地文件 {path} 删除失败");
                return false;
            }
        }

        //创建目录失败
        public override bool Mkdir(string dirPath)
        {
            try
            {
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                fileSystem.MkdirSync($"{WX.env.USER_DATA_PATH}/{dirPath}", true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //删除目录失败
        public override bool Rmdir(string dirPath, bool recursive = true)
        {
            try
            {
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                fileSystem.RmdirSync(dirPath, recursive);
                return true;
            }
            catch (Exception)
            {
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
                WXFileSystemManager fileSystem = WX.GetFileSystemManager();
                fileSystem = WX.GetFileSystemManager();
                string errorCode = fileSystem.AccessSync(path);
                if (!errorCode.ToLower().Contains("ok"))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //获得持久化目录
        public override string GetPersitentPath()
        {
            return WX.env.USER_DATA_PATH;
        }
    }
}
#endif
