using MediatR;
using ClientContactManager.Domain.Entities;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Application.Commands.Contacts;

public record CreateContactCommand(string Name, string Surname, string Email) : IRequest<int>;

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, int>
{
    private readonly IContactRepository _contactRepo;
    private readonly IUnitOfWork _uow;

    public CreateContactCommandHandler(IContactRepository contactRepo, IUnitOfWork uow)
    {
        _contactRepo = contactRepo;
        _uow = uow;
    }

    public async Task<int> Handle(CreateContactCommand request, CancellationToken ct)
    {
        var contact = new Contact(request.Name, request.Surname, request.Email);
        await _contactRepo.AddAsync(contact, ct);
        await _uow.SaveChangesAsync(ct);
        return contact.Id;
    }
}

public record LinkClientToContactCommand(int ContactId, int ClientId) : IRequest;

public class LinkClientToContactCommandHandler : IRequestHandler<LinkClientToContactCommand>
{
    private readonly IContactRepository _contactRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IUnitOfWork _uow;

    public LinkClientToContactCommandHandler(
        IContactRepository contactRepo, IClientRepository clientRepo, IUnitOfWork uow)
    {
        _contactRepo = contactRepo;
        _clientRepo = clientRepo;
        _uow = uow;
    }

    public async Task Handle(LinkClientToContactCommand request, CancellationToken ct)
    {
        var contact = await _contactRepo.GetByIdWithClientsAsync(request.ContactId, ct)
            ?? throw new KeyNotFoundException($"Contact with ID {request.ContactId} not found.");
        var client = await _clientRepo.GetByIdAsync(request.ClientId, ct)
            ?? throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        contact.LinkClient(client);
        await _uow.SaveChangesAsync(ct);
    }
}

public record UnlinkClientFromContactCommand(int ContactId, int ClientId) : IRequest;

public class UnlinkClientFromContactCommandHandler : IRequestHandler<UnlinkClientFromContactCommand>
{
    private readonly IContactRepository _contactRepo;
    private readonly IUnitOfWork _uow;

    public UnlinkClientFromContactCommandHandler(IContactRepository contactRepo, IUnitOfWork uow)
    {
        _contactRepo = contactRepo;
        _uow = uow;
    }

    public async Task Handle(UnlinkClientFromContactCommand request, CancellationToken ct)
    {
        var contact = await _contactRepo.GetByIdWithClientsAsync(request.ContactId, ct)
            ?? throw new KeyNotFoundException($"Contact with ID {request.ContactId} not found.");

        contact.UnlinkClient(request.ClientId);
        await _uow.SaveChangesAsync(ct);
    }
}
