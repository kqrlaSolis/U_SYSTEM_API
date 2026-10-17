// Ejecuta los validadores de FluentValidation del argumento de cada acción y lanza ValidationException.
// Así el middleware traduce los códigos i18n (el ModelState automático perdería el código).
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace USystem.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext Context,
        ActionExecutionDelegate Next)
    {
        foreach (var Argument in Context.ActionArguments.Values)
        {
            if (Argument is null)
            {
                continue;
            }

            var ValidatorType = typeof(IValidator<>).MakeGenericType(Argument.GetType());
            if (Context.HttpContext.RequestServices.GetService(ValidatorType) is not IValidator Validator)
            {
                continue;
            }

            var ValidationContext = new ValidationContext<object>(Argument);
            var Result = await Validator.ValidateAsync(ValidationContext);

            if (!Result.IsValid)
            {
                throw new ValidationException(Result.Errors);
            }
        }

        await Next();
    }
}
