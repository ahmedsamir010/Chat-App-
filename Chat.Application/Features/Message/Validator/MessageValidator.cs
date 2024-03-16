using Chat.Application.Features.Message.Command.AddMessage;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
