using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using MongoDB.Driver;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.ScanHeights;
using Transaction.Monitor.TransactionHistorys;

namespace Transaction.Monitor.MongoDB;

[ConnectionStringName("Default")]
public class MonitorMongoDbContext : AbpMongoDbContext
{

    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    
    public IMongoCollection<GuardKey> GuardKeys => Collection<GuardKey>();
    public IMongoCollection<AddressConfig> AddressConfigs => Collection<AddressConfig>();
    public IMongoCollection<TransactionHistory> TransactionHistorys => Collection<TransactionHistory>();
    public IMongoCollection<ScanHeight> ScanHeights => Collection<ScanHeight>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //builder.Entity<YourEntity>(b =>
        //{
        //    //...
        //});
    }
}
