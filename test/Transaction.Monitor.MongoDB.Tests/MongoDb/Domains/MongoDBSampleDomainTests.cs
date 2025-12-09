using Transaction.Monitor.Samples;
using Xunit;

namespace Transaction.Monitor.MongoDB.Domains;

[Collection(MonitorTestConsts.CollectionDefinitionName)]
public class MongoDBSampleDomainTests : SampleDomainTests<MonitorMongoDbTestModule>
{

}
