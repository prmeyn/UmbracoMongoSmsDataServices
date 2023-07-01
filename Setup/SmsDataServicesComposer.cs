using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using UmbracoMongoDbClient.Setup;

namespace UmbracoMongoSmsDataServices.Setup
{
	[ComposeAfter(typeof(MongoDbComposer))]
	public sealed class SmsDataServicesComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.AddComponent<SmsDataServicesComponent>();
		}
	}
}
