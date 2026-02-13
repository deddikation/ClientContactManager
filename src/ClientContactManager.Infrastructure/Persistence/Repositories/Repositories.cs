using Microsoft.EntityFrameworkCore;
using ClientContactManager.Domain.Entities;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _db;
    public ClientRepository(AppDbContext db) => _db = db;

    public async Task<Client?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Clients.FindAsync(new object[] { id }, ct);

    public async Task<Client?> GetByIdWithContactsAsync(int id, CancellationToken ct = default)
        => await _db.Clients
            .Include(c => c.ClientContacts).ThenInclude(cc => cc.Contact)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<Client>> GetAllAsync(CancellationToken ct = default)
        => await _db.Clients.Include(c => c.ClientContacts).AsNoTracking().ToListAsync(ct);

    public async Task<bool> ClientCodeExistsAsync(string clientCode, CancellationToken ct = default)
        => await _db.Clients.AnyAsync(c => c.ClientCode == clientCode, ct);

    public async Task AddAsync(Client client, CancellationToken ct = default)
        => await _db.Clients.AddAsync(client, ct);
}

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _db;
    public ContactRepository(AppDbContext db) => _db = db;

    public async Task<Contact?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Contacts.FindAsync(new object[] { id }, ct);

    public async Task<Contact?> GetByIdWithClientsAsync(int id, CancellationToken ct = default)
        => await _db.Contacts
            .Include(c => c.ClientContacts).ThenInclude(cc => cc.Client)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<Contact>> GetAllAsync(CancellationToken ct = default)
        => await _db.Contacts.Include(c => c.ClientContacts).AsNoTracking().ToListAsync(ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await _db.Contacts.AnyAsync(c => c.Email == email, ct);

    public async Task AddAsync(Contact contact, CancellationToken ct = default)
        => await _db.Contacts.AddAsync(contact, ct);
}
