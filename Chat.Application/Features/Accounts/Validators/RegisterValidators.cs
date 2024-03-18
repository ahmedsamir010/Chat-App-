using Chat.Application.Features.Accounts.Command.Register;
using FluentValidation;
namespace Chat.Application.Features.Accounts.Validators
{
    public class RegisterValidators : AbstractValidator<RegisterDto>
    {
        public RegisterValidators()
        {

            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("First name required")
                .NotEmpty().WithMessage("First name should not be empty")
                .MinimumLength(3).WithMessage("Minimum length for first name is {PropertyValue}");

            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Last name required")
                .NotEmpty().WithMessage("Last name should not be empty")
                .MinimumLength(3).WithMessage("Minimum length for last name is {PropertyValue}");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email address required")
                .NotEmpty().WithMessage("Email address should not be empty")
                .EmailAddress().WithMessage("Invalid email address format");
            RuleFor(x => x.Password)
                      .NotNull().WithMessage("Password required")
                      .NotEmpty().WithMessage("Password should not be empty")
                      .MinimumLength(6).WithMessage("Minimum length for password is {PropertyValue}");


  //          RuleFor(x => x.Password)
  //.NotNull().WithMessage("Password required")
  //        .NotEmpty().WithMessage("Password should not be empty")
  //      .MinimumLength(6).WithMessage("Minimum length for password is {PropertyValue}")
  //           .Matches(@"^(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter")
  //      .Matches(@"^(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter")
  //         .Matches(@"^(?=.*\d)").WithMessage("Password must contain at least one digit");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender required")
                .NotEmpty().WithMessage("Gender should not be empty");
            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth required")
                .Must(BeAValidDate).WithMessage("Invalid date of birth")
                .Must(BeUnderMaxDate).WithMessage("Date of birth must be on or before 18 years ago");

        }
        private bool BeAValidDate(DateTime date)
        {
            return date <= DateTime.Now;
        }
        private bool BeUnderMaxDate(DateTime date)
        {
            DateTime maxDate = DateTime.Now.AddYears(-18);
            return date <= maxDate;
        }

    }
}
