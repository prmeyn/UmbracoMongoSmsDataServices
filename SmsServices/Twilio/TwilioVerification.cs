using System;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

namespace UmbracoMongoSmsDataServices.SmsServices.TwilioService
{
	public sealed class TwilioVerification : IServiceMobileNumbers
	{
		private static string _accountSid;
		private static string _authToken;
		private static string _serviceSid;
		private static string _registeredSenderPhoneNumber;
		public TwilioVerification(string accountSid, string authToken, string serviceSid, string registeredSenderPhoneNumber)
		{
			ID = Guid.NewGuid();
			_accountSid = accountSid;
			_authToken = authToken;
			_serviceSid = serviceSid;
			_registeredSenderPhoneNumber = registeredSenderPhoneNumber;
		}
		public Guid ID { get; init; }

		public bool SendOTP(MobileNumber mobileWithCountryCode, string languageISOCode)
		{
			try
			{
				TwilioClient.Init(_accountSid, _authToken);
				var verification = VerificationResource.Create(
					to: $"+{mobileWithCountryCode.ToLong()}",
					channel: "sms",
					locale: languageISOCode,
					pathServiceSid: _serviceSid
				);
				return !string.IsNullOrEmpty(verification?.Sid);
			}
			catch { return false; }
		}

		public bool VerifyOTP(MobileNumber mobileWithCountryCode, string OTP)
		{
			bool verified = false;
			try
			{
				TwilioClient.Init(_accountSid, _authToken);
				var verification = VerificationCheckResource.Create(
					to: $"+{mobileWithCountryCode.ToLong()}",
					code: OTP,
					pathServiceSid: _serviceSid
				);
				verified = verification?.Valid ?? false;
				if(verified)
				{
					var countryPhoneCodeItem = SmsDataService.GetCountryCodePhoneItem(mobileWithCountryCode.GetCountryPhoneCode());
					SmsDataService.UpdateValidLengthForCountry(countryPhoneCodeItem.CountryCode, mobileWithCountryCode.Length());
				}
			}
			catch { }
			return verified;
		}

		public bool SendSMS(MobileNumber mobileWithCountryCode, string shortMessageServiceMessage)
		{
			try
			{
				var to = $"+{mobileWithCountryCode.ToLong()}";
				var too = new Twilio.Types.PhoneNumber(to);
				TwilioClient.Init(_accountSid, _authToken);
				return !string.IsNullOrWhiteSpace(MessageResource.Create(
					body: shortMessageServiceMessage,
					from: new Twilio.Types.PhoneNumber(_registeredSenderPhoneNumber),
					to: too
				)?.Sid);
			}
			catch (ApiException e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine($"Twilio Error {e.Code} - {e.MoreInfo}");
				return false;
			}
		}
	}
}
