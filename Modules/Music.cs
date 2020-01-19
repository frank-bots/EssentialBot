using cqhttp.Cyan.Events.CQEvents.Base;
using cqhttp.Cyan.Messages;
using cqhttp.Cyan.Messages.CQElements;

namespace EssentialBot.Modules {
    public class Music : Module {
        [OnCommand ("listen")]
        [OnCommand ("点歌")]
        public Message Listen (string cmd, string[] parameters, MessageEvent e) =>
            new Message (new ElementMusic (
                "163", parameters[0]
            ));
    }
}