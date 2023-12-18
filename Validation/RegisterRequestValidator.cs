using FluentValidation;
using WebApi.Models.Users;

namespace WebApi.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(model => model.FirstName).NotNull().NotEmpty().WithMessage("Please specify a name");
            RuleFor(model => model.FirstName).Length(2, 100).WithMessage("First Name must be bigger than two");
            RuleFor(model => model.LastName).NotNull().NotEmpty().WithMessage("Please specify a Last Name");
            RuleFor(model => model.LastName).Length(2, 100).WithMessage("Last Name must be bigger than two");
            RuleFor(model => model.Username).NotNull().NotEmpty().WithMessage("Please specify a Username");
            RuleFor(model => model.Username).Length(2, 100).WithMessage("User Name must be bigger than two");
            RuleFor(model => model.Password).NotNull().NotEmpty().WithMessage("Please specify a Password");
            RuleFor(model => model.Password).Length(2, 100).WithMessage("Password must be bigger than two");
        }
    }
}
