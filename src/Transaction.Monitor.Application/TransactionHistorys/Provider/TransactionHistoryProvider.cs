using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.Monitor.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Transaction.Monitor.TransactionHistorys.Provider;

public interface ITransactionHistoryProvider :
    ICrudAppService<
        TransactionHistoryDto,
        Guid,
        PagedAndSortedResultRequestDto,
        TransactionHistoryDto>
{
    Task<List<TransactionHistoryDto>> UndoneList();

    Task<bool> UpdateStatus(TransactionHistoryDto input);

    Task<bool> BatchInsert(List<TransactionHistoryDto> input);
}

public class TransactionHistoryProvider :
    CrudAppService<
        TransactionHistory,
        TransactionHistoryDto,
        Guid,
        PagedAndSortedResultRequestDto,
        TransactionHistoryDto>,
    ITransactionHistoryProvider
{
    public TransactionHistoryProvider(IRepository<TransactionHistory, Guid> repository)
        : base(repository)
    {
    }

    public async Task<List<TransactionHistoryDto>> UndoneList()
    {
        var list = await Repository.GetListAsync(p => p.Status == 0) ?? new List<TransactionHistory>();
        return list.Select(ConvertHelper.TransactionHistoryToDto).ToList();
    }

    public async Task<bool> UpdateStatus(TransactionHistoryDto input)
    {
        List<TransactionHistory> list =
            await Repository.GetListAsync(p => p.Tx == input.Tx);

        foreach (var transactionHistory in list)
        {
            transactionHistory.Status = input.Status;
            transactionHistory.RetryTimes += 1;
            await Repository.UpdateAsync(transactionHistory);
        }

        return true;
    }

    public async Task<bool> BatchInsert(List<TransactionHistoryDto> input)
    {
        if (input?.Count == 0)
        {
            return false;
        }

        List<TransactionHistory> dbs = input.Select(ConvertHelper.TransactionHistoryFromDto).ToList();
        await Repository.InsertManyAsync(dbs);

        return true;
    }
}