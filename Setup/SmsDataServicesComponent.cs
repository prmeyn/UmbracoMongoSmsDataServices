using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using UmbracoMongoSmsDataServices.SmsServices.Fake;
using UmbracoMongoSmsDataServices.SmsServices.TwilioService;

namespace UmbracoMongoSmsDataServices.Setup
{
	public sealed class SmsDataServicesComponent : IComponent
	{
		private readonly IHostingEnvironment _env;
		private readonly IConfiguration _config;
		private readonly  ILogger<SmsDataServicesComponent> _logger;
		private readonly ILocalizationService _localizationService;

		public SmsDataServicesComponent(IHostingEnvironment env, IConfiguration config, ILogger<SmsDataServicesComponent> logger, ILocalizationService localizationService)
		{
			_env = env;
			_config = config;
			_logger = logger;
			_localizationService = localizationService;
		}

		public void Initialize()
		{
			var smsDataServiceConfigPath = "SmsDataService";
			var useUmbracoLogsAsSmsInboxInNonProd = _config.GetValue<bool>($"{smsDataServiceConfigPath}:UseUmbracoLogsAsSmsInboxInNonProd");
			if (!_env.IsProduction() && useUmbracoLogsAsSmsInboxInNonProd)
			{
				SmsDataService.AddIVerifyMobileNumbers(new FakeVerification(_logger));
			}
			var twilioConfigPath = $"{smsDataServiceConfigPath}:Twilio";
			SmsDataService.AddIVerifyMobileNumbers(new TwilioVerification(
				accountSid: _config.GetValue<string>($"{twilioConfigPath}:AccountSid"),
				authToken: _config.GetValue<string>($"{twilioConfigPath}:AuthToken"),
				serviceSid: _config.GetValue<string>($"{twilioConfigPath}:ServiceSid"),
				registeredSenderPhoneNumber: _config.GetValue<string>($"{twilioConfigPath}:RegisteredSenderPhoneNumber")
			));

			SmsDataService.LoadCountryPhoneData();
			var allLanguagesWithTwoLetterIsoCodes = _localizationService.GetAllLanguages().Where(lang => !string.IsNullOrWhiteSpace(lang.IsoCode) && lang.IsoCode.Length == 2);
		}

		public void Terminate()
		{
		}
	}
}
