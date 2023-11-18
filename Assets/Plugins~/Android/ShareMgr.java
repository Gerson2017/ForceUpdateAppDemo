package com.unity3d.player;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;

import java.io.File;

import android.support.v4.content.FileProvider;
import android.util.Log;

import com.bytedance.applog.AppLog;
import com.bytedance.applog.InitConfig;
import com.bytedance.applog.util.UriConstants;

public class ShareMgr {
    public static void Share(Activity activity, String title, String path) {
        try {
            activity.runOnUiThread(()->{
                Intent imageIntent = new Intent(Intent.ACTION_SEND);
                
				if(path!=null&&path.length()!=0){
					imageIntent.setType("image/*");
					 Uri uri = getUri(activity, "quicksdk_packName.fileprovider", new File(path));
					imageIntent.putExtra(Intent.EXTRA_STREAM, uri);
				}else{
					imageIntent.setType("text/plain");
					imageIntent.putExtra(Intent.EXTRA_SUBJECT, title);
					  imageIntent.putExtra("sms_body", title);//短信时使用
					  imageIntent.putExtra("Kdescription", title);//微信朋友圈专用
				    imageIntent.putExtra(Intent.EXTRA_TEXT, title);
				}
                imageIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                activity.startActivity(Intent.createChooser(imageIntent, "Share"));
            });
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static Uri getUri(Context context, String authorites, File file) {
        Uri uri;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
            //设置7.0以上共享文件，分享路径定义在xml/file_paths.xml
            uri = FileProvider.getUriForFile(context, authorites, file);
        } else {
            // 7.0以下,共享文件
            uri = Uri.fromFile(file);
        }
        return uri;
    }

    public static void InitAppLog(Activity activity) {
        final InitConfig config = new InitConfig("557032", "tianad");
        // 设置数据上送地址
        config.setUriConfig(UriConstants.DEFAULT);
        config.setImeiEnable(false);//建议关停获取IMEI（出于合规考虑）
        config.setAutoTrackEnabled(true); // 全埋点开关，true开启，false关闭
        config.setLogEnable(true); // true:开启日志，参考4.3节设置logger，false:关闭日志
        AppLog.setEncryptAndCompress(true); // 加密开关，true开启，false关闭
        config.setAndroidIdEnabled(false);
        config.setMacEnable(false);
        config.setEnablePlay(true);
        AppLog.init(activity, config, activity);
        /* 初始化SDK结束 */
        Log.d("Unity", "Tatlk InitLog: ");
    }
}
