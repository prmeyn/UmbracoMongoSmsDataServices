using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace UmbracoMongoSmsDataServices
{
	public class SerializableContactNumber
	{
		[JsonProperty("CountryCodeAndPhoneCode")]
		public string CountryCodeAndPhoneCode { get; set; }
		[JsonProperty("ContactNumber")]
		public string ContactNumber { get; set; }

		public string FormattedContactNumber(char sepratorChar)
		{
			if (string.IsNullOrWhiteSpace(ContactNumber) || string.IsNullOrWhiteSpace(CountryCodeAndPhoneCode))
			{
				return "Error: Incorrect phone number";
			}
			return "+" + CountryCodeAndPhoneCode?.Split('#')?.LastOrDefault() + sepratorChar + Regex.Replace(ContactNumber, @"\s+", "");
		}
	}
}