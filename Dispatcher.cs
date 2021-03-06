using System;
using System.Collections.Generic;
using System.Linq;
using cqhttp.Cyan.Clients;
using cqhttp.Cyan.Enums;
using cqhttp.Cyan.Events.CQEvents.Base;
using EssentialBot.Modules;

namespace EssentialBot.Dispatcher {
    public struct Command {
        public string operation;
        /// <summary>
        /// 发送者信息
        /// </summary>
        public Sender sender;
        /// <summary>
        /// 表示从何处接收到的消息
        /// </summary>
        public (CQApiClient, (MessageType, long)) endPoint;
        public List<string> parameters;
    }
    class CommandErrorException : Exception { };
    public class Dispatcher {
        public static void Dispatch (
            CQApiClient cli,
            MessageEvent e
        ) {
            try {
                string raw_text = "";
                foreach (var i in e.message.data)
                    if (i.type == "text") raw_text += i.data["text"];
                var command = ParseCommand (raw_text);
                if (Module.loaded_modules.Values.All ((mod) => {
                        var result = mod.InvokeCommand (command.Item1, command.Item2, e);
                        if (result.data.Count == 0)
                            return true;
                        cli.SendMessageAsync (e.GetEndpoint (), result);
                        return false;
                    })) { throw new CommandErrorException (); }
            } catch (CommandErrorException) {
                foreach (var i in Module.loaded_modules.Values) {
                    var ret = i.InvokeMessage (e.message, e);
                    if (ret.data.Count != 0) //can respond
                        cli.SendMessageAsync (e.GetEndpoint (), ret);
                }
            }
        }
        static (string, string[]) ParseCommand (string raw) {
            try {
                if (raw.Split (' ') [0][0] != '/')
                    throw new CommandErrorException ();
            } catch {
                throw new CommandErrorException ();
            }
            string command = raw.Split (' ') [0].Substring (1);
            List<string> parameters = new List<string> ();
            if (raw.TrimEnd ().Length == command.Length)
                return (command, parameters.ToArray ());
            raw = raw.Substring (command.Length + 1).Trim ();
            for (int i = 0; i < raw.Length;) {
                int x = -1;
                if (raw[0] != '"') raw = raw.Insert (0, " ");
                switch (raw[i]) {
                case '"':
                    x = raw.Substring (i + 1).IndexOf ('"');
                    if (x == -1) throw new CommandErrorException ();
                    parameters.Add (raw.Substring (i + 1, x - i));
                    raw = raw.Substring (x + 2).Trim ();
                    i = 0;
                    break;
                case ' ':
                    x = raw.Substring (i + 1).Trim ().IndexOf (' ');
                    if (x == -1) {
                        parameters.Add (raw.Substring (i + 1).Trim ());
                        i = raw.Length; //break,break!
                    } else {
                        parameters.Add (raw.Substring (i + 1, x - i));
                        raw = raw.Substring (x + 1).Trim ();
                        i = 0;
                    }
                    break;
                }
            }
            return (command, parameters.ToArray ());
        }
    }
}