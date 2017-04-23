using Remotion.Linq.Parsing.Structure;
using SqlDocStore.MsSql.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SqlDocStore.MsSql.Tests
{
    public class MsSqlQueryExecutorTests
    {

        [Fact]
        public void linq_queryable_returns_simple_result()
        {
            var queryParser = QueryParser.CreateDefault();
            
            var queryable = new MsSqlJsonQueryable<SimpleDoc>(queryParser, new MsSqlQueryExecutor());
            var results = from i in queryable select i;
            
            var list = results.ToList();
            //todo: finish test
        }

        [Fact]
        public async Task queryable_returns_correct_result()
        {
            var sqlQueryParser = new MsSqlQueryParser();
            var queryExecutor = new MsSqlQueryExecutor();
            var queryable = new MsSqlJsonQueryable<SimpleDoc>(sqlQueryParser, queryExecutor);

            Expression<Func<SimpleDoc, bool>> expression = x => x.Id == 1;
            var test3 = queryable.Where(x => x.Id == 1 && x.Id == 2);
            //todo: finish test
        }
    }
}
