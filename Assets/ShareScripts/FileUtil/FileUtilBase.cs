namespace SharePublic
{
    public abstract class FileUtilBase
    {
        //同步读取文本文件
        public abstract string LoadFileTextSync(string path);

        //同步读取数据文件
        public abstract byte[] LoadFileBytesSync(string path);

        //保文本文件
        public abstract void SaveFileText(string path, string saveText);

        //同步保存文本文件
        public abstract bool SaveFileTextSync(string path, string saveText);

        //保存数据文件
        public abstract void SaveFileBytes(string path, byte[] saveBytes);

        //同步保存数据文件
        public abstract bool SaveFileBytesSync(string path, byte[] saveBytes);

        //追加数据内容
        public abstract void AppendFile(string path, byte[] saveBytes);

        //追加文本内容
        public abstract void AppendFile(string path, string saveText);

        //同步追加数据内容
        public abstract bool AppendFileSync(string path, byte[] saveBytes);

        //同步追加文本内容
        public abstract bool AppendFileSync(string path, string saveText);

        //删除文件失败
        public abstract bool DeleteFile(string path);

        //创建目录失败
        public abstract bool Mkdir(string dirPath);

        //删除目录失败
        public abstract bool Rmdir(string dirPath, bool recursive = true);

        //文件是否存在
        public abstract bool IsExist(string path);

        //获得持久化目录
        public abstract string GetPersitentPath();
    }
}

