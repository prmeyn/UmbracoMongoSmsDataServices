# Setup procedure

The `appsettings.json` file should contain your settings for your Twilio account
```json
{
	"SmsDataService": {
		"UseUmbracoLogsAsSmsInboxInNonProd": true,
		"Twilio": {
			"AccountSid": "AC863c300bf379c5b3d6365cbba1d73e15",
			"AuthToken": "7203dbcbf6fe33040b7f51a8b3daf950",
			"ServiceSid": "77dAbbe22d1fa8V2ed9dd9de555cd3ab50",
			"RegisteredSenderPhoneNumber": "+45999999"
		}
	}
}
```
Sample code
```csharp
using UmbracoMongoSmsDataServices;

IEnumerable<CountryPhoneCodeItem> CountryPhoneCodes = SmsDataService.GetAllCountryPhoneCodes.OrderBy(cpc => cpc.CountryCode);

private static MobileNumber FormatedMobileNumber(string phone_code, string contact_number) => $"+{phone_code}{MobileNumber.seprator}{contact_number}";

MobileNumber mobileNumber = FormatedMobileNumber("45", "99945349"); // For an Danish mobile number.

SmsDataService.SmsServices.First().SendOTP(
					mobileWithCountryCode: mobileNumber,
					languageISOCode: "en");
					
SmsDataService.SmsServices.First().VerifyOTP(
				mobileWithCountryCode: mobileNumber,
				OTP: "848333"));
				
SmsDataService.SmsServices.First().SendSMS(mobileNumber, "Hello World!");

```
