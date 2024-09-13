using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Transaction.Monitor.ScanHeights.Provider;

public interface IScanHeightProvider :
    ICrudAppService<
        ScanHeightDto,
        Guid,
        PagedAndSortedResultRequestDto,
        ScanHeightDto>
{
    Task<long> GetHeight(string chainId);

    Task<bool> UpdateHeight(string chainId, long height);
}

public class ScanHeightProvider :
    CrudAppService<
        ScanHeight,
        ScanHeightDto,
        Guid,
        PagedAndSortedResultRequestDto,
        ScanHeightDto>,
    IScanHeightProvider
{
    public ScanHeightProvider(IRepository<ScanHeight, Guid> repository)
        : base(repository)
    {
        
    }

    public async Task<long> GetHeight(string chainId)
    {
        ScanHeight height = await Repository.FindAsync(p => p.ChainId == chainId);
        if (null == height)
        {
            return 0;
        }

        return height.Height;
    }

    public async Task<bool> UpdateHeight(string chainId, long h)
    {
        ScanHeight height = await Repository.FindAsync(p => p.ChainId == chainId);
        if (null == height)
        {
            await Repository.InsertAsync(new ScanHeight { ChainId = chainId, Height = h }, autoSave: true);
            return true;
        }

        height.Height = h;
        await Repository.UpdateAsync(height);
        return true;
    }
}