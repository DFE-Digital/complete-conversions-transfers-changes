﻿namespace Dfe.Complete.Application.Common.Queries
{
    public class PagingQuery<T>(int page, int count) : IQueryObject<T>
    {
        private readonly int _skip = page * count;

        public IQueryable<T> Apply(IQueryable<T> query) =>
            query.Skip(_skip).Take(count);
    }
}
