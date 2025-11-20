using WINIT.Models;

namespace WINIT.Services;

public record ValidationError(
    string Field,
    string Reason
);
public record ValidationResult(
    bool isValid,
    List<ValidationError> Errors
);
public interface IIngestionService
{
    ValidationResult ValidateCustomer(IncomingCustomer customer);
    ValidationResult ValidateItem(IncomingMaterial material);

    string GenerateReferenceId(string prefix);
}