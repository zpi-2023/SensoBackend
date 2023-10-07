using FluentValidation;
using JetBrains.Annotations;

namespace SensoBackend.Application.Modules.Users.CreateUser;

[UsedImplicitly]
public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(r => r.Dto.Name).NotEmpty().MaximumLength(50);
    }
}
