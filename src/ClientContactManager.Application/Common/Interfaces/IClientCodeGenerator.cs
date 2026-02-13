namespace ClientContactManager.Application.Common.Interfaces;

public interface IClientCodeGenerator
{
    Task<string> GenerateAsync(string clientName, CancellationToken ct = default);
}
