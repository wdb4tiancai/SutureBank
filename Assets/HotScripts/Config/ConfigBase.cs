
using Luban;

namespace Game.Data
{
    public abstract class ConfigBase
    {
        public string ConfigName { get; protected set; }//表的名字
        public abstract void LoadByteBuf(ByteBuf _buf);//加载配置表数据
    }
}