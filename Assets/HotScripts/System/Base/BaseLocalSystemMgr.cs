using LitJson;
using SharePublic;

namespace Game.Mgr
{

    public class BaseLocalSystemMgr<T> : DYBaseDataMgr<T> where T : BaseSystemData, new()
    {

        private string m_FileName = string.Empty;
        private string m_SaveFileName = string.Empty;
        private bool m_IsSaveFile = false;
        private bool m_DirtyTag = false;

        protected override void InitImp()
        {
        }

        protected override void ResetImp()
        {
        }

        protected override void UpdateImp()
        {
            TrySaveData();
        }

        #region 本地记录
        //读取数据
        private void LoadLocalData()
        {
            if (!m_IsSaveFile || string.IsNullOrEmpty(m_SaveFileName))
            {
                return;
            }

            string loadDataStr = ShareFileUtil.Instance.LoadFileTextSync(m_SaveFileName);
            if (string.IsNullOrEmpty(loadDataStr))
            {
                return;
            }

            try
            {
                m_GameData = JsonMapper.ToObject<T>(loadDataStr);
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError("本地记录内容解析失败 " + m_SaveFileName + " 内容 " + loadDataStr);
            }
        }

        //保存数据。脏标记
        protected void SaveLocalData()
        {
            if (!m_IsSaveFile)
            {
                return;
            }
            m_DirtyTag = true;
        }

        //保存数据，实际逻辑
        private void TrySaveData()
        {
            if (!m_DirtyTag) { return; }
            m_DirtyTag = false;
            if (!m_IsSaveFile || string.IsNullOrEmpty(m_SaveFileName))
            {
                UnityEngine.Debug.LogError($"保存文件失败 SaveFileName {m_SaveFileName} FileName {m_FileName}");
                return;
            }
            string saveDataStr = GetLocalDataStr();
            try
            {
                ShareFileUtil.Instance.SaveFileText(m_SaveFileName, saveDataStr);
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.Log("保存本地记录失败 " + m_SaveFileName + " 内容 " + saveDataStr);
            }
        }

        private string GetLocalDataStr()
        {
            string str = string.Empty;
            try
            {
                return JsonMapper.ToJson(m_GameData); ;
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.Log("保存本地记录内容to json  str失败 " + m_SaveFileName + " 内容 " + str);
                return string.Empty;
            }
        }

        //设置文件名字
        protected void SetFileName(string filePath)
        {
            m_FileName = filePath;
            m_SaveFileName = $"{m_FileName}";
            m_IsSaveFile = true;
        }


        #endregion

    }
}
