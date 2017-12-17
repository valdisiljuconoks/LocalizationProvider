using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.DataAnnotations;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations
{
    // Credits: https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.DataAnnotations/Internal/DataAnnotationsModelValidator.cs
    public class LocalizedModelValidator : IModelValidator
    {
        private readonly ValidationAttribute _attribute;
        private static readonly object _emptyValidationContextInstance = new object();

        public LocalizedModelValidator(ValidationAttribute attribute)
        {
            _attribute = attribute;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext validationContext)
        {
            if(validationContext == null)
                throw new ArgumentNullException(nameof(validationContext));

            if(validationContext.ModelMetadata == null)
                throw new ArgumentException($"{nameof(validationContext.ModelMetadata)} is null", nameof(validationContext));

            if(validationContext.MetadataProvider == null)
                throw new ArgumentException($"{nameof(validationContext.MetadataProvider)} in null", nameof(validationContext));

            var metadata = validationContext.ModelMetadata;
            var memberName = metadata.PropertyName;
            var container = validationContext.Container;

            var context = new ValidationContext(
                container ?? validationContext.Model ?? _emptyValidationContextInstance,
                validationContext.ActionContext?.HttpContext?.RequestServices,
                null)
            {
                DisplayName = metadata.GetDisplayName(),
                MemberName = memberName
            };

            var result = _attribute.GetValidationResult(validationContext.Model, context);
            if(result != ValidationResult.Success)
            {
                var resourceKey = ResourceKeyBuilder.BuildResourceKey(metadata.ContainerType, metadata.PropertyName, _attribute);
                var translation = ModelMetadataLocalizationHelper.GetTranslation(resourceKey);
                var errorMessage = !string.IsNullOrEmpty(translation) ? translation : result.ErrorMessage;

                var validationResults = new List<ModelValidationResult>();
                if(result.MemberNames != null)
                    foreach(var resultMemberName in result.MemberNames)
                    {
                        // ModelValidationResult.MemberName is used by invoking validators (such as ModelValidator) to
                        // append construct the ModelKey for ModelStateDictionary. When validating at type level we
                        // want the returned MemberNames if specified (e.g. "person.Address.FirstName"). For property
                        // validation, the ModelKey can be constructed using the ModelMetadata and we should ignore
                        // MemberName (we don't want "person.Name.Name"). However the invoking validator does not have
                        // a way to distinguish between these two cases. Consequently we'll only set MemberName if this
                        // validation returns a MemberName that is different from the property being validated.
                        var newMemberName = string.Equals(resultMemberName, memberName, StringComparison.Ordinal)
                            ? null
                            : resultMemberName;
                        var validationResult = new ModelValidationResult(newMemberName, errorMessage);

                        validationResults.Add(validationResult);
                    }

                if(validationResults.Count == 0)
                    validationResults.Add(new ModelValidationResult(null, errorMessage));

                return validationResults;
            }

            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}
