using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniFramework.Event;

public class LauncherUI : MonoBehaviour
{
    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject _cloneObject;
        private Text _content;
        private Button _btnOK;
        private System.Action _clickOK;

        public bool ActiveSelf
        {
            get
            {
                return _cloneObject.activeSelf;
            }
        }

        public void Create(GameObject cloneObject)
        {
            _cloneObject = cloneObject;
            _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            _btnOK.onClick.AddListener(OnClickYes);
        }
        public void Show(string content, System.Action clickOK)
        {
            _content.text = content;
            _clickOK = clickOK;
            _cloneObject.SetActive(true);
            _cloneObject.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _content.text = string.Empty;
            _clickOK = null;
            _cloneObject.SetActive(false);
        }
        private void OnClickYes()
        {
            _clickOK?.Invoke();
            Hide();
        }
    }


    private readonly EventGroup _eventGroup = new EventGroup();
    private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

    // UGUI相关
    private GameObject _messageBoxObj;
    private Slider _slider;
    private Text _tips;


    void Awake()
    {
        _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
        _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
        _tips.text = "Initializing the game world !";
        _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
        _messageBoxObj.SetActive(false);

        _eventGroup.AddListener<LauncherEventDefine.InitializeFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.LauncherStatesChange>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
        _eventGroup.AddListener<LauncherEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
    }
    void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    /// <summary>
    /// 接收事件
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is LauncherEventDefine.InitializeFailed)
        {
            System.Action callback = () =>
            {
                LauncherEventDefine.UserTryInitialize.SendEventMessage();
            };
            ShowMessageBox($"补丁包初始化失败 !", callback);
        }
        else if (message is LauncherEventDefine.LauncherStatesChange)
        {
            var msg = message as LauncherEventDefine.LauncherStatesChange;
            _tips.text = msg.Tips;
        }
        else if (message is LauncherEventDefine.FoundUpdateFiles)
        {
            var msg = message as LauncherEventDefine.FoundUpdateFiles;
            System.Action callback = () =>
            {
                LauncherEventDefine.UserBeginDownloadWebFiles.SendEventMessage();
            };
            float sizeMB = msg.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"发现更新文件, 更新文件数量 {msg.TotalCount} 更新文件大小 {totalSizeMB}MB", callback);
        }
        else if (message is LauncherEventDefine.DownloadProgressUpdate)
        {
            var msg = message as LauncherEventDefine.DownloadProgressUpdate;
            _slider.value = (float)msg.CurrentDownloadCount / msg.TotalDownloadCount;
            string currentSizeMB = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            _tips.text = $"{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
        else if (message is LauncherEventDefine.PackageVersionUpdateFailed)
        {
            System.Action callback = () =>
            {
                LauncherEventDefine.UserTryUpdatePackageVersion.SendEventMessage();
            };
            ShowMessageBox($"更新静态版本失败，请检查网络状态.", callback);
        }
        else if (message is LauncherEventDefine.PatchManifestUpdateFailed)
        {
            System.Action callback = () =>
            {
                LauncherEventDefine.UserTryUpdatePatchManifest.SendEventMessage();
            };
            ShowMessageBox($"更新补丁清单失败，请检查网络状态.", callback);
        }
        else if (message is LauncherEventDefine.WebFileDownloadFailed)
        {
            var msg = message as LauncherEventDefine.WebFileDownloadFailed;
            System.Action callback = () =>
            {
                LauncherEventDefine.UserTryDownloadWebFiles.SendEventMessage();
            };
            ShowMessageBox($"下载文件失败 : {msg.FileName}", callback);
        }
        else
        {
            throw new System.NotImplementedException($"{message.GetType()}");
        }
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    private void ShowMessageBox(string content, System.Action ok)
    {
        // 尝试获取一个可用的对话框
        MessageBox msgBox = null;
        for (int i = 0; i < _msgBoxList.Count; i++)
        {
            var item = _msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }

        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            _msgBoxList.Add(msgBox);
        }

        // 显示对话框
        msgBox.Show(content, ok);
    }
}