using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Base;
using ChatOnlineApi.Infrastructure.Persistence.Contexts;

namespace ChatOnlineApi.Infrastructure.Persistence.Repositories;

public class MessageReadReceiptRepository (ChatOnlineDbContext _db ) : GenericRepository<MessageReadReceipt>(_db), IMessageReadReceiptRepository
{
}
