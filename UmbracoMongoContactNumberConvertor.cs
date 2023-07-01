using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace UmbracoMongoSmsDataServices
{
	public sealed class UmbracoMongoContactNumberConvertor : IPropertyValueConverter
	{
		public bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias == "UmbracoMongoContactNumber";
		public object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) => inter as SerializableContactNumber;

		public object? ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
		{
			throw new NotImplementedException();
		}

		public object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
		{
			if (IsValid(source, out SerializableContactNumber? number))
			{
				return number;
			}
			return null;
		}

		public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Element;

		public Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(SerializableContactNumber);
		public bool? IsValue(object? value, PropertyValueLevel level) => IsValid(value, out SerializableContactNumber? _);

		private bool IsValid(object? value, out SerializableContactNumber? number)
		{
			var serializedContactNumber = value as string;
			if (string.IsNullOrWhiteSpace(serializedContactNumber))
			{
				number = null;
				return false;
			}
			number = JsonConvert.DeserializeObject<SerializableContactNumber>(serializedContactNumber);
			return number != null && !string.IsNullOrWhiteSpace(number.CountryCodeAndPhoneCode) && !string.IsNullOrWhiteSpace(number.ContactNumber);
		}
	}
}
