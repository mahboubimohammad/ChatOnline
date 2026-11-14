using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Base;
using ChatOnlineApi.Infrastructure.Persistence.Contexts;

namespace ChatOnlineApi.Infrastructure.Persistence.Repositories;

public class ConversationTypeRepository(ChatOnlineDbContext _db) : GenericRepository<ConversationType>(_db), IConversationTypeRepository
{
}
