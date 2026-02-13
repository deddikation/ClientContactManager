using ClientContactManager.Application.Common.Interfaces;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Infrastructure.Services;

/// <summary>
/// Generates unique 6-char alphanumeric client codes.
/// Format: 3 uppercase alpha (from client name) + 3 sequential digits.
/// Examples: "First National Bank" -> FNB001, "Protea" -> PRO001, "IT" -> ITA001
/// </summary>
public class ClientCodeGenerator : IClientCodeGenerator
{
    private readonly IClientRepository _clientRepo;
    public ClientCodeGenerator(IClientRepository clientRepo) => _clientRepo = clientRepo;

    public async Task<string> GenerateAsync(string clientName, CancellationToken ct = default)
    {
        var prefix = BuildAlphaPrefix(clientName);

        for (int i = 1; i <= 999; i++)
        {
            var code = $"{prefix}{i:D3}";
            if (!await _clientRepo.ClientCodeExistsAsync(code, ct))
                return code;
        }

        throw new InvalidOperationException(
            $"All client codes exhausted for prefix '{prefix}'.");
    }

    private static string BuildAlphaPrefix(string clientName)
    {
        // Take first 3 alpha characters from the name, uppercased
        var chars = clientName
            .Where(char.IsLetter)
            .Take(3)
            .Select(char.ToUpper)
            .ToList();

        // If name has fewer than 3 letters, pad with A, B, C...
        var pad = 'A';
        while (chars.Count < 3)
        {
            chars.Add(pad);
            pad++;
        }

        return new string(chars.ToArray());
    }
}
