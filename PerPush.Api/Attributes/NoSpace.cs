﻿using System.ComponentModel.DataAnnotations;

namespace PerPush.Api.Attributes
{
    public class NoSpace : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string val)
            {
                return !val.Contains(" ");
            }
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"The {validationContext.DisplayName} can not contain space!");
            }
        }
    }
}
