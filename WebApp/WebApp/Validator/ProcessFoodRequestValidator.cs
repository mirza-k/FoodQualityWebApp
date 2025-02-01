using FluentValidation;
using Models.Enum;
using Models.Request;

namespace WebApp.Validator
{
    public class ProcessFoodRequestValidator : AbstractValidator<ProcessFoodRequest>
    {
        public ProcessFoodRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required.")
                .MaximumLength(100).WithMessage("Serial number cannot exceed 100 characters.");

            RuleFor(x => x.TypeOfAnalysis)
                        .Must(value => Enum.IsDefined(typeof(AnalysisType), value))
                        .WithMessage("Invalid TypeOfAnalysis. Allowed values: 1 (MICROBIOLOGICAL_ANALAYSIS), 2 (CHEMICAL_ANALAYSIS).");
        }
    }
}
