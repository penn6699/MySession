using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// 请求助手
/// </summary>
public class WebRequestHelper
{
    public WebRequestHelper(){}

    /// <summary>
    /// post方式提交或获取网络数据
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="strPostdata">发送的数据</param>
    /// <returns></returns>
    public static string Post(string url, Dictionary<string, object> param = null)
    {
        try
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (param != null)
            {
                string strPostdata = "";
                foreach (KeyValuePair<string, object> kvp in param)
                {
                    if (!string.IsNullOrEmpty(kvp.Key))
                    {
                        strPostdata += (strPostdata == "" ? "" : "&") + string.Format("{0}={1}", kvp.Key, kvp.Value.ToString());
                    }
                }
                if (strPostdata != "")
                {
                    //往服务器写入数据
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strPostdata);
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }

            }

            //获取服务端返回
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception exp)
        {
            throw new Exception("后台post方式提交或获取网络数据错误。" + exp.Message);
        }
    }


}