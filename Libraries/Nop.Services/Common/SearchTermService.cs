using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Services.Common
{
    /// <summary>
    /// Search term service
    /// </summary>
    public partial class SearchTermService : ISearchTermService
    {
        #region Fields

        private readonly IRepository<SearchTerm> _searchTermRepository;

        #endregion

        #region Ctor

        public SearchTermService(IRepository<SearchTerm> searchTermRepository)
        {
            _searchTermRepository = searchTermRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a search term record by keyword
        /// </summary>
        /// <param name="keyword">Search term keyword</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search term
        /// </returns>
        public virtual async Task<SearchTerm> GetSearchTermByKeywordAsync(string keyword, int storeId, int customerId = 0, int yearId = 0, int makeId = 0, int modelId = 0, int engineId = 0)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;

            var query = from st in _searchTermRepository.Table
                        where st.Keyword == keyword && st.StoreId == storeId
                        orderby st.Id
                        select st;

            if (customerId > 0)
                query = query.Where(x => x.CustomerId == customerId).OrderBy(st => st.Id);

            if (yearId > 0)
                query = query.Where(x => x.YearId == yearId).OrderBy(st => st.Id);

            if (makeId > 0)
                query = query.Where(x => x.MakeId == makeId).OrderBy(st => st.Id);

            if (modelId > 0)
                query = query.Where(x => x.ModelId == modelId).OrderBy(st => st.Id);

            if (engineId > 0)
                query = query.Where(x => x.EngineId == engineId).OrderBy(st => st.Id);

            var searchTerm = await query.FirstOrDefaultAsync();

            return searchTerm;
        }

        /// <summary>
        /// Gets a search term statistics
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a list search term report lines
        /// </returns>
        public virtual async Task<IPagedList<SearchTermReportLine>> GetStatsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _searchTermRepository.Table;
            var data = query.Select(x => new { x }).GroupBy(x => new { x.x.YearId, x.x.MakeId, x.x.ModelId, x.x.EngineId, x.x.Keyword }).Select(x => new SearchTermReportLine()
            {
                Keyword = x.FirstOrDefault().x.Keyword,
                Year = x.FirstOrDefault().x.YearId,
                Make = x.FirstOrDefault().x.MakeId,
                Model = x.FirstOrDefault().x.ModelId,
                Engine = x.FirstOrDefault().x.EngineId,
                Count = x.Select(x => x.x.Count).Sum()
            }).OrderByDescending(x => x.Count);

            //var query = (from st in _searchTermRepository.Table
            //             group st by st.Keyword into groupedResult
            //             select new
            //             {
            //                 Keyword = groupedResult.Key,
            //                 Count = groupedResult.Sum(o => o.Count)
            //             })
            //            .OrderByDescending(m => m.Count)
            //            .Select(r => new SearchTermReportLine
            //            {
            //                Keyword = r.Keyword,
            //                Count = r.Count
            //            });

            var result = await data.ToPagedListAsync(pageIndex, pageSize);

            return result;
        }

        /// <summary>
        /// Inserts a search term record
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSearchTermAsync(SearchTerm searchTerm)
        {
            await _searchTermRepository.InsertAsync(searchTerm);
        }

        /// <summary>
        /// Updates the search term record
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSearchTermAsync(SearchTerm searchTerm)
        {
            await _searchTermRepository.UpdateAsync(searchTerm);
        }

        #endregion
    }
}