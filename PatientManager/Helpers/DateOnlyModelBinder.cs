using Microsoft.AspNetCore.Mvc.ModelBinding;

public class DateOnlyModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (DateOnly.TryParse(value, out var dateOnly))
        {
            bindingContext.Result = ModelBindingResult.Success(dateOnly);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format.");
        }

        return Task.CompletedTask;
    }
}
