using FluentValidation;
namespace Chat.Application.Features.Message.Validator
{
    public class MessageValidator : AbstractValidator<AddMessageDto>
    {
        public MessageValidator()
        {
            RuleFor(x => x.contentMessage).NotNull().WithMessage("{PropertyName} is Not Null")
                .MinimumLength(3).WithMessage("{PropertyName} is Min Length {PropertyValue}");


            RuleFor(x=>x.recipentUserName).NotNull().WithMessage("{PropertyName} is required!")
                                             .MinimumLength(3).WithMessage("{PropertyName} is Min Length {PropertyValue}");
                
        }
    }
}
