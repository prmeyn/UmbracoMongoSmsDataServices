using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using UmbracoMongoSmsDataServices.SmsServices.Fake;
using UmbracoMongoSmsDataServices.SmsServices.Twilio;

namespace UmbracoMongoSmsDataServices.Setup
{
	public class SmsDataServicesComponent : IComponent
	{
		private readonly IHostingEnvironment _env;
		private readonly IConfiguration _config;
		private readonly  ILogger<SmsDataServicesComponent> _logger;

		public SmsDataServicesComponent(IHostingEnvironment env, IConfiguration config, ILogger<SmsDataServicesComponent> logger)
		{
			_env = env;
			_config = config;
			_logger = logger;
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
		}

		public void Terminate()
		{
		}
	}
}
