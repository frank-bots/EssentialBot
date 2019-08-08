using cqhttp.Cyan.Messages;
using cqhttp.Cyan.Messages.CQElements;

namespace EssentialBot.Functions {
    public class Music {
        public static void LoadModule () {
            FunctionPool.onCommand.Add ("listen", (p) => new Message (new ElementMusic (
                "163", p.parameters[0]
            )));
            FunctionPool.onCommand.Add ("点歌", (p) => new Message (new ElementMusic (
                "163", p.parameters[0]
            )));
        }
        public static void UnloadModule () {
            FunctionPool.onCommand.Remove ("listen");
            FunctionPool.onCommand.Remove ("点歌");
        }
    }
}