using FluentValidation;
using JetBrains.Annotations;

namespace SensoBackend.Application.Modules.Users.CreateUser;

[UsedImplicitly]
public sealed class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
