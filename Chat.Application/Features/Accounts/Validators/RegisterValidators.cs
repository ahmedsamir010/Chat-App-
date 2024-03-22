using Chat.Application.Features.Accounts.Command.Register;
using FluentValidation;
namespace Chat.Application.Features.Accounts.Validators
{
    public class RegisterValidators : AbstractValidator<RegisterDto>
    {
        public RegisterValidators()
        {
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("First name is required")
                .NotEmpty().WithMessage("First name should not be empty")
                .MinimumLength(3).WithMessage("Minimum length for first name is {MinLength}");

            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Last name is required")
                .NotEmpty().WithMessage("Last name should not be empty")
                .MinimumLength(3).WithMessage("Minimum length for last name is {MinLength}");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email address is required")
                .NotEmpty().WithMessage("Email address should not be empty")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Password is required")
                .NotEmpty().WithMessage("Password should not be empty")
                .MinimumLength(6).WithMessage("Minimum length for password is {MinLength}");

            //          RuleFor(x => x.Password)
            //.NotNull().WithMessage("Password required")
            //        .NotEmpty().WithMessage("Password should not be empty")
            //      .MinimumLength(6).WithMessage("Minimum length for password is {PropertyValue}")
            //           .Matches(@"^(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter")
            //      .Matches(@"^(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter")
            //         .Matches(@"^(?=.*\d)").WithMessage("Password must contain at least one digit");
            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender required")
                .NotEmpty().WithMessage("Gender should not be empty")
                .Must(gender => gender != null && (gender.ToLower() == "male" || gender.ToLower() == "female"))
                    .WithMessage("Gender should be either 'Male' or 'Female'");

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
