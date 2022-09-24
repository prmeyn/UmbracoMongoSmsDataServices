using Common.Utilities;
using Newtonsoft.Json;
using UmbracoMongoSmsDataServices;

namespace UmbracoMongoSmsDataServices
{
	public static class ContactNumberUtility
	{
		public static string? GetFormatted(string serializedContactNumber)
		{
			var number = JsonConvert.DeserializeObject<SerializableContactNumber>(serializedContactNumber);
			return number?.FormattedContactNumber(' ');
		}
		public static string ComputeSha256HashOfFormattedNumber(SerializableContactNumber serializableContactNumber) => CryptoUtils.ComputeSha256Hash(GetFormatted(JsonConvert.SerializeObject(serializableContactNumber)));
	}
}
