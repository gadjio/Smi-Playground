namespace Smi.Infra.Extensions
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public static class DataAnnotationValidatorExtensions
	{
		public static bool Validate(this object obj, out List<ValidationResult> results)
		{
			ValidationContext context = new ValidationContext(obj, null, null);
			results = new List<ValidationResult>();

			return Validator.TryValidateObject(obj, context, results, true);
		}
	}
}