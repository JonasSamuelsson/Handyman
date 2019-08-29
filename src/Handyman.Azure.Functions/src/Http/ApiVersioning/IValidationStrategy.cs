namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public interface IValidationStrategy
    {
        bool Validate(ValidationContext validationContext);
    }
}