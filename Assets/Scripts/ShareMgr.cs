using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_WEBGL
using WeChatWASM;
#endif


public class ShareMgr
{
    AndroidJavaObject currentActivity;
    AndroidJavaClass shareClass;

    public ShareMgr()
    {
        if (Application.isEditor)
            return;

        AndroidJavaClass ac = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = ac.GetStatic<AndroidJavaObject>("currentActivity");
        shareClass = new AndroidJavaClass("com.unity3d.player.ShareMgr");
    }

    public void Share(string path, string content)
    {
        if (Application.isEditor)
            return;
        shareClass.CallStatic("Share", currentActivity, content, path);
    }


    public void InitAppLog()
    {
        if (Application.isEditor)
            return;
        shareClass.CallStatic("InitAppLog", currentActivity);
        Debug.Log("InitAppLog");
    }

}

public class Entry : MonoBehaviour
{
    public Camera rtCamera;

    void Start()
    {
        StartCoroutine(Share());
    }


    IEnumerator Share()
    {
        yield return new WaitForSeconds(2);
        rtCamera.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        var path = SaveRenderTextureToPNG(rtCamera.targetTexture, "123");
        yield return new WaitForEndOfFrame();
        rtCamera.gameObject.SetActive(false);
        new ShareMgr().Share(path, "分享 https://www.baidu.com");
    }

    public static string SaveRenderTextureToPNG(RenderTexture rt, string pngName)
    {
        Debug.Log("正在保存PNG图片...." + pngName);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        byte[] bytes = png.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, pngName + ".png");

        Debug.Log(path);

        FileStream file = File.Open(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        DestroyImmediate(png);
        png = null;

        RenderTexture.active = prev;

        return path;
    }


    public class WXShare
    {
        public static void Share(string title)
        {
            int ShareHeight = UnityEngine.Screen.height / 3;
#if  UNITY_WEBGL

            WXCanvas.ToTempFilePath(new WXToTempFilePathParam()
                {
                    x = (Screen.width-ShareHeight) / 2,
                    y = ShareHeight,
                    width = ShareHeight,
                    height = ShareHeight,
                    destWidth = ShareHeight,
                    destHeight = ShareHeight,
                    success = (result) =>
                    {
                        Debug.Log("ToTempFilePath success" + JsonUtility.ToJson(result));
                        WX.ShareAppMessage(new ShareAppMessageOption()
                        {
                            title = title,
                            imageUrl = result.tempFilePath,
                        });
                    },
                    fail = (result) =>
                    {
                        Debug.Log("ToTempFilePath fail" + JsonUtility.ToJson(result));
                    },
                    complete = (result) =>
                    {
                        Debug.Log("ToTempFilePath complete" + JsonUtility.ToJson(result));
                    },
                });
            
#endif
        }

        public static void RegisterShare(string title, string imagePath)
        {
#if  UNITY_WEBGL
            WX.OnShareAppMessage(new WXShareAppMessageParam()
            {
                title = title,
                imageUrl = imagePath,
            });
#endif
        }


        public static void CheckUpdate()
        {
#if UNITY_WEBGL
            WXUpdateManager _updateManager = WX.GetUpdateManager();
            MyDebuger.Log("检查新版本!");
            _updateManager.OnCheckForUpdate((res) =>
                {
                    MyDebuger.Log("新版本已准备好 "+res.hasUpdate);
                }
            );
            ShowModalOption sucessshareinfo = new ShowModalOption();
            sucessshareinfo.title = "更新提示";
            sucessshareinfo.content = "新版本已经准备好，是否重启应用？";
            sucessshareinfo.success = (res) =>
            {
                if (res.confirm) {
                    // 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
                    _updateManager.ApplyUpdate();
                }
            };
            
            
            ShowModalOption failshareinfo = new ShowModalOption();
            failshareinfo.title = "更新失败提示";
            failshareinfo.content = "下载失败，请删除当前小程序后重新打开";
            failshareinfo.showCancel = false;
            failshareinfo.confirmText = "知道了";
            _updateManager.OnUpdateReady((res) => { WX.ShowModal(sucessshareinfo);});

            _updateManager.OnUpdateFailed((res) =>
            {
                WX.ShowModal(failshareinfo);
            });
#endif
        }

    }
}
