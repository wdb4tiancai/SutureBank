
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using Cysharp.Threading.Tasks;

namespace Game.Data
{
    public partial class GameConfigs
    {
        public Tips Tips { get; private set; }
        public UiCfg UiCfg { get; private set; }

        public async UniTask LoadRes(System.Func<string, UniTask<ByteBuf>> loader)
        {
            ByteBuf loadData = null;
            loadData = await loader("tips");
            Tips = new Tips(loadData);
            loadData = await loader("uicfg");
            UiCfg = new UiCfg(loadData);
            ResolveRef();
        }
        private void ResolveRef()
        {
            Tips.ResolveRef(this);
            UiCfg.ResolveRef(this);
        }
    }

}
