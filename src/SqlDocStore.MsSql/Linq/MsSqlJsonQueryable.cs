using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using System.Linq;
using System.Linq.Expressions;

namespace SqlDocStore.MsSql.Linq
{
    public class MsSqlJsonQueryable<T> : QueryableBase<T>
    {
        public MsSqlJsonQueryable(IQueryParser queryParser, IQueryExecutor executor)
        : base(new MsSqlQueryProvider(typeof(MsSqlJsonQueryable<>), queryParser, executor))
        {

        }

        public MsSqlJsonQueryable(IQueryProvider provider, Expression expression)
        : base(provider, expression)
        {
        }
    }
}
