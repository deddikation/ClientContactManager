using ClientContactManager.Domain.Entities;

namespace ClientContactManager.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Client?> GetByIdWithContactsAsync(int id, CancellationToken ct = default);
    Task<List<Client>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ClientCodeExistsAsync(string clientCode, CancellationToken ct = default);
    Task AddAsync(Client client, CancellationToken ct = default);
}

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Contact?> GetByIdWithClientsAsync(int id, CancellationToken ct = default);
    Task<List<Contact>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task AddAsync(Contact contact, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
