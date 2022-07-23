using System;
using System.Linq;

namespace UmbracoMongoSmsDataServices.SmsServices
{
	public struct MobileNumber
	{
		private long longValue;
		private string stringValue;
		private int numberLength;
		private string countryMobileCodeString;
		private string localMobileNumberString;

		public static readonly char seprator = '-';
		public MobileNumber(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) { throw new Exception($"Invalid MobileNumber>>{value}<<"); }
			if (!value.Contains(seprator)) { throw new Exception($"Invalid MobileNumber>>{value}<< requires '{seprator}' between the country code and mobile number"); }
			var mobileNumberArray = value.Split(seprator);
			if (mobileNumberArray.Length != 2) { throw new Exception($"Invalid MobileNumber>>{value}<< expected length 2 found {mobileNumberArray.Length}"); }
			countryMobileCodeString = GetNumbers(mobileNumberArray[0]);
			localMobileNumberString = GetNumbers(mobileNumberArray[1]);
			numberLength = localMobileNumberString.Length;
			if (!int.TryParse(countryMobileCodeString, out int countryMobileCode)) { throw new Exception($"Invalid countryCode {countryMobileCode} in MobileNumber>>{value}<<"); }
			if (!long.TryParse(localMobileNumberString, out long localMobileNumber)) { throw new Exception($"Invalid localMobileNumber {localMobileNumber} in MobileNumber>>{value}<<"); }
			this.stringValue = $"{countryMobileCode}{seprator}{localMobileNumber}";
			long.TryParse($"{countryMobileCode}{localMobileNumber}", out long longValue);
			this.longValue = longValue;
		}

		public int Length()
		{
			return this.numberLength;
		}
		public string GetCountryPhoneCode() => countryMobileCodeString;
		public string GetPhoneNumber() => localMobileNumberString;

		private static string GetNumbers(string input)
		{
			return new string(input.Where(c => char.IsDigit(c)).ToArray());
		}
		public override bool Equals(object obj)
		{
			if (obj is MobileNumber tokenIdentifier)
			{
				return this.Equals(tokenIdentifier);
			}

			return false;
		}
		public override int GetHashCode()
		{
			return longValue.GetHashCode();
		}
		public bool Equals(MobileNumber other)
		{
			return this.longValue == other.longValue && this.stringValue == other.stringValue;
		}
		
		public override string ToString()
		{
			return this.stringValue;
		}

		public long ToLong()
		{
			return this.longValue;
		}

		public static implicit operator MobileNumber(string value)
		{
			return new MobileNumber(value);
		}

		public static explicit operator string(MobileNumber tokenIdentifier)
		{
			return tokenIdentifier.stringValue;
		}


		public static bool operator ==(MobileNumber left, MobileNumber right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MobileNumber left, MobileNumber right)
		{
			return !left.Equals(right);
		}
	}
}
