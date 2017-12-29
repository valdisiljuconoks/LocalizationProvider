using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    // Credits: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.DataAnnotations/Internal/DataAnnotationsMetadataProvider.cs
    public class LocalizedValidationMetadataProvider : IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            for(var i = 0; i < context.Results.Count; i++)
            {
                var validatorItem = context.Results[i];
                if(validatorItem.Validator != null && !(validatorItem.Validator is DataAnnotationsModelValidator))
                    continue;

                var attribute = validatorItem.ValidatorMetadata as ValidationAttribute;
                if(attribute == null)
                    continue;

                validatorItem.Validator = new LocalizedModelValidator(attribute);
                validatorItem.IsReusable = true;

                // Inserts validators based on whether or not they are 'required'. We want to run
                // 'required' validators first so that we get the best possible error message.
                if(attribute is RequiredAttribute)
                {
                    context.Results.Remove(validatorItem);
                    context.Results.Insert(0, validatorItem);
                }
            }

            // Produce a validator if the type supports IValidatableObject
            if(typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
                context.Results.Add(new ValidatorItem
                {
                    Validator = new ValidatableObjectAdapter(),
                    IsReusable = true
                });
        }
    }
}
