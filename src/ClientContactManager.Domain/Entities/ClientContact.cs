namespace ClientContactManager.Domain.Entities;

// Many-to-many join entity. A contact can be linked to multiple clients and vice versa.
public class ClientContact
{
    public int ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public int ContactId { get; private set; }
    public Contact Contact { get; private set; } = null!;

    private ClientContact() { }

    public ClientContact(Client client, Contact contact)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Contact = contact ?? throw new ArgumentNullException(nameof(contact));
        ClientId = client.Id;
        ContactId = contact.Id;
    }
}
