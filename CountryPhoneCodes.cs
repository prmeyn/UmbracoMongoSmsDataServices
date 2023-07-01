using MongoDB.Bson.Serialization.Attributes;

namespace UmbracoMongoSmsDataServices
{
	public sealed class CountryPhoneCodes
	{
		public CountryPhoneCodes() {}
		public CountryPhoneCodes(KeyValuePair<string, CountryPhoneCodes> r)
		{
			CountryCode = r.Key;
			CountryPhoneCodeList = r.Value.CountryPhoneCodeList;
			ValidLengths = r.Value.ValidLengths;
		}

		[BsonId]
		public string CountryCode { get; set; }
		public string[]? CountryPhoneCodeList { get; set; }
		public int[]? ValidLengths { get; set; }
	}
	public sealed class CountryPhoneCodeItem
	{
		public string CountryPhoneCode { get; set; }
		public string CountryCode { get; set; }
		public int[]? ValidLengths { get; set; }
	}
}
