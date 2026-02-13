using MediatR;
using ClientContactManager.Application.Common.Interfaces;
using ClientContactManager.Domain.Entities;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Application.Commands.Clients;

public record CreateClientCommand(string Name) : IRequest<int>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    private readonly IClientRepository _clientRepo;
    private readonly IClientCodeGenerator _codeGenerator;
    private readonly IUnitOfWork _uow;

    public CreateClientCommandHandler(
        IClientRepository clientRepo, IClientCodeGenerator codeGenerator, IUnitOfWork uow)
    {
        _clientRepo = clientRepo;
        _codeGenerator = codeGenerator;
        _uow = uow;
    }

    public async Task<int> Handle(CreateClientCommand request, CancellationToken ct)
    {
        var clientCode = await _codeGenerator.GenerateAsync(request.Name, ct);
        var client = new Client(request.Name, clientCode);

        await _clientRepo.AddAsync(client, ct);
        await _uow.SaveChangesAsync(ct);

        return client.Id;
    }
}

public record LinkContactToClientCommand(int ClientId, int ContactId) : IRequest;

public class LinkContactToClientCommandHandler : IRequestHandler<LinkContactToClientCommand>
{
    private readonly IClientRepository _clientRepo;
    private readonly IContactRepository _contactRepo;
    private readonly IUnitOfWork _uow;

    public LinkContactToClientCommandHandler(
        IClientRepository clientRepo, IContactRepository contactRepo, IUnitOfWork uow)
    {
        _clientRepo = clientRepo;
        _contactRepo = contactRepo;
        _uow = uow;
    }

    public async Task Handle(LinkContactToClientCommand request, CancellationToken ct)
    {
        var client = await _clientRepo.GetByIdWithContactsAsync(request.ClientId, ct)
            ?? throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        var contact = await _contactRepo.GetByIdAsync(request.ContactId, ct)
            ?? throw new KeyNotFoundException($"Contact with ID {request.ContactId} not found.");

        client.LinkContact(contact);
        await _uow.SaveChangesAsync(ct);
    }
}

public record UnlinkContactFromClientCommand(int ClientId, int ContactId) : IRequest;

public class UnlinkContactFromClientCommandHandler : IRequestHandler<UnlinkContactFromClientCommand>
{
    private readonly IClientRepository _clientRepo;
    private readonly IUnitOfWork _uow;

    public UnlinkContactFromClientCommandHandler(IClientRepository clientRepo, IUnitOfWork uow)
    {
        _clientRepo = clientRepo;
        _uow = uow;
    }

    public async Task Handle(UnlinkContactFromClientCommand request, CancellationToken ct)
    {
        var client = await _clientRepo.GetByIdWithContactsAsync(request.ClientId, ct)
            ?? throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");

        client.UnlinkContact(request.ContactId);
        await _uow.SaveChangesAsync(ct);
    }
}
