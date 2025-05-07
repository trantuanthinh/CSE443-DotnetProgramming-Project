using Project.AppContext;
using Project.Core.Base.Repository;
using Project.Models;

namespace Project.Repositories
{
    public class ConversationRepository(DataContext context)
        : BaseRepository<Conversation, DataContext>(context) { }

    public class MessageRepository(DataContext context)
        : BaseRepository<Message, DataContext>(context) { }
}
