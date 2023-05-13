using Common.Utilities;
using Newtonsoft.Json;

namespace UmbracoMongoSmsDataServices
{
	public static class ContactNumberUtility
	{
		public static string? GetFormatted(string serializedContactNumber)
		{
			var number = JsonConvert.DeserializeObject<SerializableContactNumber>(serializedContactNumber);
			return number?.FormattedContactNumber(' ');
		}
		public static string ComputeSha512HashOfFormattedNumber(SerializableContactNumber serializableContactNumber) => CryptoUtils.ComputeSha512Hash(GetFormatted(JsonConvert.SerializeObject(serializableContactNumber)));
	}
}
