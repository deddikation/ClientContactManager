namespace ClientContactManager.Domain.Entities;

public class Contact
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public string FullName => $"{Surname} {Name}";

    private readonly List<ClientContact> _clientContacts = new();
    public IReadOnlyCollection<ClientContact> ClientContacts => _clientContacts.AsReadOnly();

    private Contact() { }

    public Contact(string name, string surname, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(surname))
            throw new ArgumentException("Surname cannot be empty.", nameof(surname));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        Name = name;
        Surname = surname;
        Email = email;
    }

    public void LinkClient(Client client)
    {
        if (_clientContacts.Any(cc => cc.ClientId == client.Id))
            throw new InvalidOperationException($"Client '{client.Name}' is already linked to this contact.");

        _clientContacts.Add(new ClientContact(client, this));
    }

    public void UnlinkClient(int clientId)
    {
        var link = _clientContacts.FirstOrDefault(cc => cc.ClientId == clientId)
            ?? throw new InvalidOperationException("Client is not linked to this contact.");

        _clientContacts.Remove(link);
    }
}
