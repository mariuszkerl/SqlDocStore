namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Linq;
    using Sql;
    using Xunit;
    using System.Linq.Expressions;
    using System.Collections;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.Structure;
    using Remotion.Linq.Clauses;
    using Remotion.Linq.Clauses.ExpressionVisitors;
    using Remotion.Linq.Parsing;
    using SqlDocStore.MsSql.Linq.ExpressionVisitors;
    using System.Text;
    using SqlDocStore.MsSql.Linq.QueryBuilders;

    public class MsSqlExpressionVisitorTest
    {
        [Fact]
        public async Task visitor_builds_binary_constant_query()
        {
            var visitor = new MsSqlExpressionVisitor(new JsonQueryBuilder());

            Expression<Func<SimpleDoc, bool>> expression = x => x.Id == 1;
            visitor.Visit(expression.Body);

            var query = visitor.QueryBuilder.Query.ToString();

            Assert.Equal("JSON_VALUE(ColumnWithDocument, '$.Id')=@0", query);
        }

        [Fact]
        public async Task visitor_builds_binary_constant_and_constant_query()
        {
            var visitor = new MsSqlExpressionVisitor(new JsonQueryBuilder());

            Expression<Func<SimpleDoc, bool>> expression = x => x.Id == 1 && x.Id == 2;
            visitor.Visit(expression.Body);

            var query = visitor.QueryBuilder.Query.ToString();

            Assert.Equal("JSON_VALUE(ColumnWithDocument, '$.Id')=@0andJSON_VALUE(ColumnWithDocument, '$.Id')=@1", query);
        }

        [Fact]
        public async Task visitor_builds_binary_constant_or_constant_query()
        {
            var visitor = new MsSqlExpressionVisitor(new JsonQueryBuilder());

            Expression<Func<SimpleDoc, bool>> expression = x => x.Id == 1 || x.Id == 2;
            visitor.Visit(expression.Body);

            var query = visitor.QueryBuilder.Query.ToString();

            Assert.Equal("JSON_VALUE(ColumnWithDocument, '$.Id')=@0orJSON_VALUE(ColumnWithDocument, '$.Id')=@1", query);
        }
    }
}
