using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Reflection;
using UmbracoMongoDbClient;
using UmbracoMongoSmsDataServices.SmsServices;

namespace UmbracoMongoSmsDataServices
{
	public static class SmsDataService
	{
		private static readonly string _countryPhoneDataCollectionName = "CountryPhoneData";
		public static Dictionary<string, CountryPhoneCodes> CountryPhoneData;
		public static List<IServiceMobileNumbers> SmsServices = new();
		public static Dictionary<string, CountryPhoneCodes> GetAllCountryPhoneCodesDictionary => CountryPhoneData;
		public static IEnumerable<CountryPhoneCodeItem> GetAllCountryPhoneCodes => CountryPhoneData.Values.SelectMany(c => c.CountryPhoneCodeList.Select(cpc => new CountryPhoneCodeItem() { CountryPhoneCode = cpc, CountryCode = c.CountryCode, ValidLengths = c.ValidLengths } ));

		public static Dictionary<string, Dictionary<string, string>> CountryNames { get; internal set; }

		public static IEnumerable<CountryPhoneCodeWithNameItem> GetAllCountryPhoneCodesWithNames(string langaugeTwoLetterIsoCode) {
			var translatedCountryNames = CountryNames[langaugeTwoLetterIsoCode];
			return GetAllCountryPhoneCodes.Select(c => new CountryPhoneCodeWithNameItem(c) {
				CountryName = translatedCountryNames[c.CountryCode]
			});
		}
		public static void LoadCountryPhoneData()
		{
			var database = MongoDBClientConnection.GetDatabase(MethodBase.GetCurrentMethod().DeclaringType.Name);
			var collection = database.GetCollection<BsonDocument>(_countryPhoneDataCollectionName);

			if (collection.CountDocuments("{}") == 0)
			{
				var dictionaryOfCountryPhoneCodes = CountryPhoneCodeData.Load();
				collection.InsertMany(dictionaryOfCountryPhoneCodes.Select(r => new CountryPhoneCodes(r).ToBsonDocument()));
			}
			CountryPhoneData = collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync().Result.ToDictionary(v => v.GetValue(0).ToString(), v => BsonSerializer.Deserialize<CountryPhoneCodes>(v));

			new Thread(WatchForCollectionChanges).Start();
		}
		public static void AddIVerifyMobileNumbers(IServiceMobileNumbers SmsService) => SmsServices.Add(SmsService);
		private static void WatchForCollectionChanges()
		{
			var database = MongoDBClientConnection.GetDatabase(MethodBase.GetCurrentMethod().DeclaringType.Name);
			var collection = database.GetCollection<BsonDocument>(_countryPhoneDataCollectionName);
			using (var cursor = collection.Watch())
			{
				foreach (var change in cursor.ToEnumerable())
				{
					CountryPhoneData = collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync().Result.ToDictionary(v => v.GetValue(0).ToString(), v => BsonSerializer.Deserialize<CountryPhoneCodes>(v));
					break;
				}
			}
		}

		public static void UpdateValidLengthForCountry(string countryIsoCode, int validContactNumberLength)
		{
			if (CountryPhoneData.TryGetValue(countryIsoCode, out CountryPhoneCodes value) && !value.ValidLengths.Contains(validContactNumberLength))
			{
				var database = MongoDBClientConnection.GetDatabase(MethodBase.GetCurrentMethod().DeclaringType.Name);
				var collection = database.GetCollection<BsonDocument>(_countryPhoneDataCollectionName);
				value.ValidLengths = value.ValidLengths.Append(validContactNumberLength).ToArray();
				collection.ReplaceOne(
					filter: Builders<BsonDocument>.Filter.Eq("_id", countryIsoCode),
					value.ToBsonDocument());
			}
		}

		public static CountryPhoneCodeItem GetCountryCodePhoneItem(string phone_code)
		{
			return GetAllCountryPhoneCodes.FirstOrDefault(p => p.CountryPhoneCode == phone_code);
		}

		public static MobileNumber FormatedMobileNumber(string phone_code, string contact_number) => $"+{phone_code}{MobileNumber.seprator}{contact_number}";

	}
}
