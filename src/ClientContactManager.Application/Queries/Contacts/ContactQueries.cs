using MediatR;
using ClientContactManager.Application.DTOs;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Application.Queries.Contacts;

public record GetAllContactsQuery : IRequest<List<ContactListDto>>;

public class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, List<ContactListDto>>
{
    private readonly IContactRepository _contactRepo;
    public GetAllContactsQueryHandler(IContactRepository contactRepo) => _contactRepo = contactRepo;

    public async Task<List<ContactListDto>> Handle(GetAllContactsQuery request, CancellationToken ct)
    {
        var contacts = await _contactRepo.GetAllAsync(ct);
        return contacts
            .OrderBy(c => c.FullName)
            .Select(c => new ContactListDto(c.Id, c.Name, c.Surname, c.Email, c.ClientContacts.Count))
            .ToList();
    }
}

public record GetContactByIdQuery(int Id) : IRequest<ContactDetailDto?>;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDetailDto?>
{
    private readonly IContactRepository _contactRepo;
    public GetContactByIdQueryHandler(IContactRepository contactRepo) => _contactRepo = contactRepo;

    public async Task<ContactDetailDto?> Handle(GetContactByIdQuery request, CancellationToken ct)
    {
        var contact = await _contactRepo.GetByIdWithClientsAsync(request.Id, ct);
        if (contact is null) return null;

        var clients = contact.ClientContacts
            .OrderBy(cc => cc.Client.Name)
            .Select(cc => new LinkedClientDto(cc.ClientId, cc.Client.Name, cc.Client.ClientCode))
            .ToList();

        return new ContactDetailDto(contact.Id, contact.Name, contact.Surname, contact.Email, clients);
    }
}

public record GetAvailableClientsForContactQuery(int ContactId) : IRequest<List<ClientListDto>>;

public class GetAvailableClientsForContactQueryHandler
    : IRequestHandler<GetAvailableClientsForContactQuery, List<ClientListDto>>
{
    private readonly IContactRepository _contactRepo;
    private readonly IClientRepository _clientRepo;

    public GetAvailableClientsForContactQueryHandler(
        IContactRepository contactRepo, IClientRepository clientRepo)
    {
        _contactRepo = contactRepo;
        _clientRepo = clientRepo;
    }

    public async Task<List<ClientListDto>> Handle(
        GetAvailableClientsForContactQuery request, CancellationToken ct)
    {
        var contact = await _contactRepo.GetByIdWithClientsAsync(request.ContactId, ct);
        var allClients = await _clientRepo.GetAllAsync(ct);
        var linkedIds = contact?.ClientContacts.Select(cc => cc.ClientId).ToHashSet() ?? new();

        return allClients
            .Where(c => !linkedIds.Contains(c.Id))
            .OrderBy(c => c.Name)
            .Select(c => new ClientListDto(c.Id, c.Name, c.ClientCode, c.ClientContacts.Count))
            .ToList();
    }
}
