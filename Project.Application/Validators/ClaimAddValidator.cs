using FluentValidation;
using Project.Domain.DTOs.Requests;

namespace Project.Application.Validators
{
    public class ClaimAddValidator : AbstractValidator<AddClaimRequestDto>
    {
        public ClaimAddValidator()
        {
            RuleFor(x => x.ProviderCode)
                .NotEmpty().WithMessage("ProviderCode is required")
                .MaximumLength(50);

            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("MemberId is required")
                .MaximumLength(50);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");
        }
    }
}
