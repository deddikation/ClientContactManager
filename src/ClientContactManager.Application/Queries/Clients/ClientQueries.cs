using MediatR;
using ClientContactManager.Application.DTOs;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Application.Queries.Clients;

public record GetAllClientsQuery : IRequest<List<ClientListDto>>;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, List<ClientListDto>>
{
    private readonly IClientRepository _clientRepo;
    public GetAllClientsQueryHandler(IClientRepository clientRepo) => _clientRepo = clientRepo;

    public async Task<List<ClientListDto>> Handle(GetAllClientsQuery request, CancellationToken ct)
    {
        var clients = await _clientRepo.GetAllAsync(ct);
        return clients
            .OrderBy(c => c.Name)
            .Select(c => new ClientListDto(c.Id, c.Name, c.ClientCode, c.ClientContacts.Count))
            .ToList();
    }
}

public record GetClientByIdQuery(int Id) : IRequest<ClientDetailDto?>;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDetailDto?>
{
    private readonly IClientRepository _clientRepo;
    public GetClientByIdQueryHandler(IClientRepository clientRepo) => _clientRepo = clientRepo;

    public async Task<ClientDetailDto?> Handle(GetClientByIdQuery request, CancellationToken ct)
    {
        var client = await _clientRepo.GetByIdWithContactsAsync(request.Id, ct);
        if (client is null) return null;

        var contacts = client.ClientContacts
            .OrderBy(cc => cc.Contact.FullName)
            .Select(cc => new LinkedContactDto(cc.ContactId, cc.Contact.FullName, cc.Contact.Email))
            .ToList();

        return new ClientDetailDto(client.Id, client.Name, client.ClientCode, contacts);
    }
}

public record GetAvailableContactsForClientQuery(int ClientId) : IRequest<List<ContactListDto>>;

public class GetAvailableContactsForClientQueryHandler
    : IRequestHandler<GetAvailableContactsForClientQuery, List<ContactListDto>>
{
    private readonly IClientRepository _clientRepo;
    private readonly IContactRepository _contactRepo;

    public GetAvailableContactsForClientQueryHandler(
        IClientRepository clientRepo, IContactRepository contactRepo)
    {
        _clientRepo = clientRepo;
        _contactRepo = contactRepo;
    }

    public async Task<List<ContactListDto>> Handle(
        GetAvailableContactsForClientQuery request, CancellationToken ct)
    {
        var client = await _clientRepo.GetByIdWithContactsAsync(request.ClientId, ct);
        var allContacts = await _contactRepo.GetAllAsync(ct);
        var linkedIds = client?.ClientContacts.Select(cc => cc.ContactId).ToHashSet() ?? new();

        return allContacts
            .Where(c => !linkedIds.Contains(c.Id))
            .OrderBy(c => c.FullName)
            .Select(c => new ContactListDto(c.Id, c.Name, c.Surname, c.Email, c.ClientContacts.Count))
            .ToList();
    }
}
