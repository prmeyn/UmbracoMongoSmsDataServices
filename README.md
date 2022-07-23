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

```csharp
using UmbracoMongoSmsDataServices;

;
```
