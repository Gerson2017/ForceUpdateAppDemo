 using System;
 using UnityEngine;

 ///<summary>
    /// 创建人：Gerson
    /// 日 期：2023/4/21 19:39:18
    /// 描 述：1.子节点通知父节点用委托或事件
    ///2.父节点调用子节点可以直接方法调用
    ///3.跨模块通信用事件
    ///4.耦合就是双向引用或循环引用
    ///</summary>
    public class TiTalkEvent: MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("TiTalkEvent Start");
        new ShareMgr().InitAppLog();
            SenderRegister();
        }

        AndroidJavaClass _obj;
        

         AndroidJavaClass EventObj
        {
            get
            {
                if (_obj==null)
                {
                    _obj = new AndroidJavaClass("com.bytedance.applog.game.GameReportHelper");
                    Debug.Log("EventObj init");
                }
                return _obj;
            }
        }

        

        //内置事件: “注册” ，属性：注册方式，是否成功，属性值为：wechat ，true GameReportHelper.onEventRegister("wechat",true);
        //内置事件 “支付”，属性：商品类型，商品名称，商品ID，商品数量，支付渠道，币种，是否成功（必传），金额（必传）GameReportHelper.onEventPurchase("gift","flower", "008",1,                "wechat","¥", true, 1);

        bool _hasregister;
 

        public  void SenderRegister()
        {
            if (_hasregister)
                return;
            if (Application.isEditor)
                return;
            if (EventObj == null)
            {
                Debug.Log("SenderRegister EventObj null");
                return;
            }
            _hasregister = true;
            EventObj.CallStatic("onEventRegister", "2113", true);
            Debug.Log("Titalk SenderRegister");
        }

        /// <summary>
        /// 是否成功（必传），金额（必传）
        /// </summary>
        public  void Paurchase(bool var6, int var7,string var0="", string var1 = "", string var2 = "", int var3=0, string var4 = "", string var5 = "")
        {
            Debug.Log("Titalk Start");
            if (Application.isEditor)
                return;
            if (EventObj == null)
            {
                Debug.Log("Titalk Paurchase EventObj null");
                return;
            }
           
            EventObj.CallStatic("onEventPurchase", var0,var1,var2,var3,var4,var5,var6,var7);
            Debug.Log("Titalk Paurchase");
        }


    }
