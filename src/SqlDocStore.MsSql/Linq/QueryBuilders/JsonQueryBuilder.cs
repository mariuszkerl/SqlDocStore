using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDocStore.MsSql.Linq.QueryBuilders
{
    public class JsonQueryBuilder
    {
        public StringBuilder Query { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public JsonQueryBuilder()
        {
            Query = new StringBuilder();
            Parameters = new Dictionary<string, object>();
        }

        public string AddParameter(object value)
        {
            var parameter = string.Concat("@", Parameters.Count);

            Parameters.Add(parameter, value);

            return parameter;
        }
    }
}
