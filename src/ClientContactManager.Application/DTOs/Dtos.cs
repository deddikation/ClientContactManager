namespace ClientContactManager.Application.DTOs;

public record ClientListDto(int Id, string Name, string ClientCode, int LinkedContactCount);

public record ClientDetailDto(int Id, string Name, string ClientCode, List<LinkedContactDto> LinkedContacts);

public record LinkedContactDto(int ContactId, string FullName, string Email);

public record ContactListDto(int Id, string Name, string Surname, string Email, int LinkedClientCount);

public record ContactDetailDto(int Id, string Name, string Surname, string Email, List<LinkedClientDto> LinkedClients);

public record LinkedClientDto(int ClientId, string ClientName, string ClientCode);
