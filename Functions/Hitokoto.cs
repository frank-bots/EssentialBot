using System;
using System.Collections.Generic;
using cqhttp.Cyan.Messages;
using Newtonsoft.Json.Linq;

namespace EssentialBot.Functions {
    public class Hitokoto {
        static List<string> types = new List<string> {
            "Anime",
            "Comic",
            "Game",
            "Novel",
            "Original",
            "Internet",
            "Other"
        };
        public static Message GetHitokoto (List<string> p) {
            JObject result = new JObject ();
            string url = "https://v1.hitokoto.cn/?encode=json&charset=utf-8";
            if (p.Count > 0) {
                string helpMsg = "Usage: /{hitokoto/一言} [parameter]\n其中:parameter可为空或 ";
                foreach (var i in types)
                    helpMsg += i + ',';
                if (p[0] == "help") return new Message (helpMsg.TrimEnd (',') + " 其中之一");
                else if (types.Contains (p[0]))
                    url += $"&c={Convert.ToChar('a' + types.IndexOf(p[0]))}";
            }
            try {
                using (var http = new System.Net.Http.HttpClient ()) {
                    result = JObject.Parse (http.GetStringAsync (url).Result);
                }
            } catch { return new Message ("网络错误"); }
            return new Message ($"{result["hitokoto"].ToString()}\n--{result["from"].ToString()}");
        }
        public static void LoadModule () {
            FunctionPool.onCommand.Add ("hitokoto", (p) => GetHitokoto (p.parameters));
            FunctionPool.onCommand.Add ("一言", (p) => GetHitokoto (p.parameters));
        }
        public static void UnloadModule () {
            FunctionPool.onCommand.Remove ("hitokoto");
            FunctionPool.onCommand.Remove ("一言");
        }
    }
}