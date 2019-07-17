using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// 会话上下文
/// </summary>
public static class MySessionContext
{
    /// <summary>
    /// 会话中存放 SessionId 的键名称
    /// </summary>
    private static string SessionIdKey = Guid.NewGuid().ToString("N");
    /// <summary>
    /// 会话中存放 Expire 的键名称
    /// </summary>
    private static string ExpireKey = Guid.NewGuid().ToString("N");
    /// <summary>
    /// 会话数据
    /// </summary>
    private static Dictionary<string, Dictionary<string, object>> SessionData = new Dictionary<string, Dictionary<string, object>>();

    #region Init

    /// <summary>
    /// 是否初始化
    /// </summary>
    private static bool IsInit = false;
    /// <summary>
    /// 会话初始化
    /// </summary>
    private static void InitSessionContext() {
        System.Timers.Timer t = new System.Timers.Timer(30000);   //实例化Timer类，设置间隔时间为30 * 1,000 毫秒；   
        t.Elapsed += new System.Timers.ElapsedEventHandler(theout); //到达时间的时候执行事件；   
        t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
        t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件； 

        IsInit = true;
    }

    private static object locker = new object();
    /// <summary>
    /// 处理过期会话数据
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private static void theout(object source, System.Timers.ElapsedEventArgs e)
    {
        lock (locker)
        {
            try
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> kvp in SessionData)
                {
                    Dictionary<string, object> _Session = kvp.Value;
                    if (_Session.ContainsKey(ExpireKey))
                    {
                        DateTime Expire;
                        try
                        {
                            Expire = (DateTime)_Session[ExpireKey];
                        }
                        catch
                        {
                            Expire = DateTime.Now;
                        }

                        TimeSpan ts = Expire - DateTime.Now;
                        if (ts.TotalSeconds <= 0)
                        {
                            SessionData.Remove(kvp.Key);
                        }
                    }
                    else
                    {
                        SessionData.Remove(kvp.Key);
                    }
                }
            }
            catch { }
        }
    }

    #endregion

    /// <summary>
    /// 超时时间。单位分钟。默认20分钟
    /// </summary>
    private static double DefaultSessionTimeout
    {
        get {
            try
            {
                return Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["SessionTimeout"]);
            }
            catch
            {
                return 20;
            }
        }
    }

    /// <summary>
    /// 当前会话
    /// </summary>
    public static MySession Current
    {
        get
        {
            if (!IsInit) {
                InitSessionContext();
            }
            
            string SessionID = HttpContext.Current.Request["MySessionID"] ?? "null";
            
            Dictionary<string, object> Session = new Dictionary<string, object>();
            if (SessionData.ContainsKey(SessionID)) {
                Session= SessionData[SessionID];
            }else{
                SessionID = Guid.NewGuid().ToString("N");

                HttpCookie cookie = new HttpCookie("MySessionID", SessionID);
                cookie.Expires = DateTime.Now.AddDays(1);

                HttpContext.Current.Response.AppendCookie(cookie);

                Session[SessionIdKey] = SessionID;
                Session[ExpireKey] = DateTime.Now.AddMinutes(DefaultSessionTimeout);

                SessionData.Add(SessionID, Session);
            }

            return new MySession(ref Session,SessionIdKey,ExpireKey);

        }
    }
    

}

/// <summary>
/// 会话
/// </summary>
public class MySession {
    private Dictionary<string,object> Data;    
    private string ExpireKey;

    public string SessionID;
    
    public MySession() { }
    public MySession(ref Dictionary<string, object> Data, string SessionIdKey, string ExpireKey) {
        this.Data = Data;
        this.ExpireKey = ExpireKey;

        SessionID = (string)Data[SessionIdKey];
    }

    /// <summary>
    /// 取消会话
    /// </summary>
    public void Abandon()
    {
        Data[ExpireKey] = DateTime.Now.AddMinutes(-3);
    }

    /// <summary>
    /// 获取或设置超时时间，单位为分钟
    /// </summary>
    public double Timeout
    {
        get
        {

            DateTime Expire = (DateTime)Data[ExpireKey];
            return (Expire - DateTime.Now).TotalMinutes;
        }
        set
        {
            Data[ExpireKey] = DateTime.Now.AddSeconds(value * 60 + 3); ;
        }
    }
    
    /// <summary>
    /// 设置键值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(string key, object value)
    {
        Data[key] = value;
    }

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsExist(string key)
    {
        return Data.ContainsKey(key);
    }

    /// <summary>
    /// 获取键值
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object Get(string key)
    {
        return IsExist(key) ? Data[key] : null;
    }
    /// <summary>
    /// 获取/设置键值
    /// </summary>
    /// <param name="key">键名称</param>
    /// <returns></returns>
    public object this[string key]
    {
        get
        {
            return Get(key);
        }
        set
        {
            Data[key] = value;
        }
    }
    
}