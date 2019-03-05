using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    /// <summary>
    /// PErform image file size validation
    /// </summary>
    public class ImageSizeValidationAttribute : ValidationAttribute
    {
        private int _maxFileSize;

        public ImageSizeValidationAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var imageFile = (IFormFile)value;

            if (imageFile != null && imageFile.Length > _maxFileSize)
            {
                return new ValidationResult(string.Format("File size limit is {0} KB", (_maxFileSize / 1000)));
            }

            return ValidationResult.Success;
        }
    }
}
