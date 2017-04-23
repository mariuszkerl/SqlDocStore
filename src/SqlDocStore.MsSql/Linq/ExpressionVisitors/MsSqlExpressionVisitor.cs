using Remotion.Linq.Parsing;
using SqlDocStore.MsSql.Linq.QueryBuilders;
using System.Linq.Expressions;
using System.Text;

namespace SqlDocStore.MsSql.Linq.ExpressionVisitors
{
    public class MsSqlExpressionVisitor: ExpressionVisitor
    {
        public JsonQueryBuilder QueryBuilder { get; protected set; }

        public string DocColumn { get; protected set; }

        public MsSqlExpressionVisitor(JsonQueryBuilder queryBuilder)
        {
            QueryBuilder = queryBuilder;
            DocColumn = "ColumnWithDocument";
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            QueryBuilder.Query.Append(QueryBuilder.AddParameter(node.Value));

            return null;
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            if ((binary.NodeType == ExpressionType.AndAlso) || (binary.NodeType == ExpressionType.OrElse))
            {
                Visit(binary.Left);
                var separator = binary.NodeType == ExpressionType.AndAlso
                    ? "and"
                    : "or";
                QueryBuilder.Query.Append(separator);
                Visit(binary.Right);
            }
            else if (binary.NodeType == ExpressionType.Equal)
            {
                Visit(binary.Left);
                QueryBuilder.Query.Append("=");
                Visit(binary.Right);
            }

            return null;
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            StringBuilder pathBuilder = new StringBuilder();
            StringBuilder jsonValueBuilder = new StringBuilder();
            var me = member;
            while (me != null)
            {
                pathBuilder.Insert(0, me.Member.Name);

                me = me.Expression as MemberExpression;
                if (me != null)
                {
                    pathBuilder.Insert(0, ".");
                }
            }

            QueryBuilder.Query.Append(member.Type.IsValueType || member.Type.Name == "String" ? "JSON_VALUE(" : "JSON_QUERY(")
                .Append(DocColumn)
                .Append(", '$.")
                .Append(pathBuilder.ToString())
                .Append("')");

            return null;
        }
    }
}
