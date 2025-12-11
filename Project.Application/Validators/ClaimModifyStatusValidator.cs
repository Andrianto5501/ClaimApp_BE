using FluentValidation;
using Project.Domain.DTOs.Requests;
using Project.Domain.Enums;

namespace Project.Application.Validators
{
    public class ClaimModifyStatusValidator : AbstractValidator<ClaimModifyStatusRequestDto>
    {
        public ClaimModifyStatusValidator()
        {
            RuleFor(x => x.ClaimId)
                .NotEmpty().WithMessage("ClaimId is required");

            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("NewStatus is required")
                .Must(BeValidStatus).WithMessage("Invalid claim status");
        }

        private bool BeValidStatus(string status)
        {
            return Enum.TryParse(typeof(ClaimStatus), status, true, out _);
        }
    }
}
