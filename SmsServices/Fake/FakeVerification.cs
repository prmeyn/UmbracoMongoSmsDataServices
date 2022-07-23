using Microsoft.Extensions.Logging;
using TokenManager;

namespace UmbracoMongoSmsDataServices.SmsServices.Fake
{
	public class FakeVerification : IServiceMobileNumbers
	{
		private readonly ILogger _logger;
		public FakeVerification(ILogger logger)
		{
			ID = Guid.NewGuid();
			_logger = logger;
		}
		public Guid ID { get; init; }

		public bool SendOTP(MobileNumber mobileWithCountryCode, string languageISOCode)
		{
			_logger.LogInformation("Sending token to {mobileWithCountryCode} : {OTP} :{languageISOCode}", mobileWithCountryCode, TokenLogic.Generate(mobileWithCountryCode.ToString(), 60 * 3, 6), languageISOCode);
			return true;
		}

		public bool VerifyOTP(MobileNumber mobileWithCountryCode, string OTP)
		{
			if (TokenLogic.Validate(mobileWithCountryCode.ToString(), OTP))
			{
				return TokenLogic.ConsumeAndValidate(mobileWithCountryCode.ToString(), OTP);
			}
			return false;
		}

		public bool SendSMS(MobileNumber mobileWithCountryCode, string shortMessageServiceMessage)
		{
			_logger.LogInformation("Sending to {mobileWithCountryCode} : Message {shortMessageServiceMessage}", mobileWithCountryCode, shortMessageServiceMessage);
			return true;
		}
	}
}
