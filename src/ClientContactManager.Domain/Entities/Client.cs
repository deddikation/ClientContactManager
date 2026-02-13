namespace ClientContactManager.Domain.Entities;

public class Client
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ClientCode { get; private set; } = string.Empty;

    private readonly List<ClientContact> _clientContacts = new();
    public IReadOnlyCollection<ClientContact> ClientContacts => _clientContacts.AsReadOnly();

    private Client() { }

    public Client(string name, string clientCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Client name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(clientCode))
            throw new ArgumentException("Client code cannot be empty.", nameof(clientCode));

        Name = name;
        ClientCode = clientCode;
    }

    public void LinkContact(Contact contact)
    {
        if (_clientContacts.Any(cc => cc.ContactId == contact.Id))
            throw new InvalidOperationException($"Contact '{contact.Email}' is already linked to this client.");

        _clientContacts.Add(new ClientContact(this, contact));
    }

    public void UnlinkContact(int contactId)
    {
        var link = _clientContacts.FirstOrDefault(cc => cc.ContactId == contactId)
            ?? throw new InvalidOperationException("Contact is not linked to this client.");

        _clientContacts.Remove(link);
    }
}
