using MongoDB.Bson.Serialization.Attributes;

namespace UmbracoMongoSmsDataServices
{
	public class CountryPhoneCodes
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
	public class CountryPhoneCodeItem
	{
		public string CountryPhoneCode { get; set; }
		public string CountryCode { get; set; }
		public int[]? ValidLengths { get; set; }
	}

	public class CountryPhoneCodeWithNameItem : CountryPhoneCodeItem
	{
		public CountryPhoneCodeWithNameItem(CountryPhoneCodeItem c)
		{
			CountryPhoneCode = c.CountryPhoneCode;
			CountryCode = c.CountryCode;
			ValidLengths = c.ValidLengths;
		}

		public string CountryName { get; set; }
	}

}
