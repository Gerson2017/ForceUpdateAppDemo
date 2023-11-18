using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System;

public class UpdateAppTool : MonoBehaviour
{

    public Button MLoadAppBtn;
    public Slider MProgressSlider;

    public string MLoadApkUrl;

    AndroidJavaClass androidJavaClass;
    AndroidJavaObject androidJavaObject;
    AndroidJavaClass customToolClass;
    AndroidJavaObject customToolObject;

    private string _serverdownLoadPath;
    private string _localPath;


    private void Start()
    {
        MLoadAppBtn.onClick.AddListener(OnUpdateBtnClick);
     
    }

    private void OnUpdateBtnClick()
    {
        CheckUpdate(MLoadApkUrl, 10);
    }


    private void Update()
    {
        MProgressSlider.value = DownLoadProgress;
    }


    public float DownLoadProgress
    {
        get
        {
            if (www != null)
                return www.downloadProgress;
            return 1;
        }
    }


    public void CheckUpdate(string loadApkUrl, int apkVersion)
    {
        _serverdownLoadPath = loadApkUrl;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //获取MainActivity的实例对象
            androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            customToolClass = new AndroidJavaClass("com.Unity.Tools.UTool"); //com.Unity.Tools 包名  UTool 类名
            customToolObject = customToolClass.CallStatic<AndroidJavaObject>("instance");
            customToolObject.Call("Init", androidJavaObject);
        }
        catch (Exception e)
        {
            Debug.LogError("CheckUpdate "+e.Data.ToString());
        }

        customToolClass = new AndroidJavaClass("com.Unity.Tools.UTool");
        customToolObject = new AndroidJavaObject("com.Unity.Tools.UTool");
        customToolObject = customToolClass.CallStatic<AndroidJavaObject>("instance");
        customToolObject.Call("Init", androidJavaObject);
#endif
     
        _localPath = $"{Application.persistentDataPath}/{apkVersion}.apk";
        Debug.Log(_localPath);
        if (File.Exists(_localPath))
            AndoridInstallApk();
        else
            StartCoroutine(InstallApk());

    }



    private UnityWebRequest www = null;

    public IEnumerator InstallApk()
    {
        www = UnityWebRequest.Get(_serverdownLoadPath);

        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("AppUpdate error:" + www.error);
            yield return new WaitForSeconds(1);
            StartCoroutine(InstallApk());
        }

        if (www.isDone)
        {
            File.WriteAllBytes(_localPath, www.downloadHandler.data);
            yield return new WaitForSeconds(1);
            AndoridInstallApk();
        }

        yield return new WaitForSeconds(7f);
        Application.Quit();

    }


    void AndoridInstallApk()
    {
        if (customToolObject != null)
            customToolObject.Call("installApk", _localPath);
        Debug.Log("AndoridInstallApk");
    }


    public void Destory()
    {
        if (File.Exists(_localPath))
            File.Delete(_localPath);
        if (www != null)
            www.Dispose();
    }

}