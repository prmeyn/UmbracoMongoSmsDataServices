using System;

namespace UmbracoMongoSmsDataServices.SmsServices
{
	public interface IServiceMobileNumbers
	{
		public Guid ID { get; init; }
		bool SendOTP(MobileNumber mobileWithCountryCode, string languageISOCode);
		bool VerifyOTP(MobileNumber mobileWithCountryCode, string OTP);
		bool SendSMS(MobileNumber mobileWithCountryCode, string shortMessageServiceMessage);
	}
}
