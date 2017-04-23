using Remotion.Linq;
using Remotion.Linq.Clauses;
using SqlDocStore.MsSql.Linq.ExpressionVisitors;
using SqlDocStore.MsSql.Linq.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDocStore.MsSql.Linq
{
    public class MsSqlQueryExecutor: IQueryExecutor
    {
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var wheres = queryModel.BodyClauses.OfType<WhereClause>().ToArray();
            if (wheres.Length == 0) { return new List<T>(); }

            var predicates = wheres.Select(x => x.Predicate);
            var visitor = new MsSqlExpressionVisitor(new JsonQueryBuilder());
            visitor.Visit(predicates.First());
            //todo: get result from db here
            return new List<T>();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);

            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            // We'll get to this one later...
            throw new NotImplementedException();
        }
    }
}
