using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ImageToPdfAPI.Models.RequestModel
{
    

    public class UserRequest
    {
        [Required(ErrorMessage = "File is required.")]
        [DataType(DataType.Upload)]
        [AllowedFileExtensions]
        public IFormFile File { get; set; }
    }

    public class AllowedFileExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] allowedExtensions = { ".tif", ".docx", ".msg",".png",".jpeg",".jpg" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

                if (allowedExtensions.Contains(fileExtension))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"Only {string.Join(", ", allowedExtensions)} file types are allowed.");
                }
            }

            return new ValidationResult("Invalid file type.");
        }
    }
}
