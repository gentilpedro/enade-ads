using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EnadeAds.Tests;

/// <summary>
/// Faz o SQLite em memória responder como o PostgreSQL: toda consulta assíncrona cede a vez antes de executar.
/// Sem isso, o <c>await</c> no banco termina na hora e a suíte não vê a página renderizada no meio da consulta.
/// </summary>
public sealed class AsyncDatabaseInterceptor : DbCommandInterceptor
{
    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return result;
    }

    public override async ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return result;
    }
}
