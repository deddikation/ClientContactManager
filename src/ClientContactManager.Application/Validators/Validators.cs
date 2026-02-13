using FluentValidation;
using ClientContactManager.Application.Commands.Clients;
using ClientContactManager.Application.Commands.Contacts;
using ClientContactManager.Domain.Interfaces;

namespace ClientContactManager.Application.Validators;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(200).WithMessage("Client name must not exceed 200 characters.");
    }
}

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    private readonly IContactRepository _contactRepo;

    public CreateContactCommandValidator(IContactRepository contactRepo)
    {
        _contactRepo = contactRepo;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MustAsync(BeUniqueEmail).WithMessage("This email address is already in use.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken ct)
        => !await _contactRepo.EmailExistsAsync(email, ct);
}
