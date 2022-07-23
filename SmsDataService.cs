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
		private static Dictionary<string, CountryPhoneCodes> _countryPhoneData;
		public static List<IServiceMobileNumbers> SmsServices = new();
		public static Dictionary<string, CountryPhoneCodes> GetAllCountryPhoneCodesDictionary => _countryPhoneData;
		public static IEnumerable<CountryPhoneCodeItem> GetAllCountryPhoneCodes => _countryPhoneData.Values.SelectMany(c => c.CountryPhoneCodeList.Select(cpc => new CountryPhoneCodeItem() { CountryPhoneCode = cpc, CountryCode = c.CountryCode, ValidLengths = c.ValidLengths } ));
		public static void LoadCountryPhoneData()
		{
			var database = MongoDBClientConnection.GetDatabase(MethodBase.GetCurrentMethod().DeclaringType.Name);
			var collection = database.GetCollection<BsonDocument>(_countryPhoneDataCollectionName);

			if (collection.CountDocuments("{}") == 0)
			{
				var dictionaryOfCountryPhoneCodes = CountryPhoneCodeData.Load();
				collection.InsertMany(dictionaryOfCountryPhoneCodes.Select(r => new CountryPhoneCodes(r).ToBsonDocument()));
			}
			_countryPhoneData = collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync().Result.ToDictionary(v => v.GetValue(0).ToString(), v => BsonSerializer.Deserialize<CountryPhoneCodes>(v));

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
					_countryPhoneData = collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync().Result.ToDictionary(v => v.GetValue(0).ToString(), v => BsonSerializer.Deserialize<CountryPhoneCodes>(v));
					break;
				}
			}
		}

		public static void UpdateValidLengthForCountry(string countryIsoCode, int validContactNumberLength)
		{
			if (_countryPhoneData.TryGetValue(countryIsoCode, out CountryPhoneCodes value) && !value.ValidLengths.Contains(validContactNumberLength))
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
	}
}
