using Transaction.Monitor.MongoDB;
using Transaction.Monitor.Samples;
using Xunit;

namespace Transaction.Monitor.MongoDb.Applications;

[Collection(MonitorTestConsts.CollectionDefinitionName)]
public class MongoDBSampleAppServiceTests : SampleAppServiceTests<MonitorMongoDbTestModule>
{

}
