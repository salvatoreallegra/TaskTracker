using FluentValidation;
using TaskTracker.Api.Dtos;

public class CreateTaskRequestValidator : AbstractValidator<TaskCreateDto>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(100).WithMessage("Task title cannot be longer than 100 characters");
    }
}
