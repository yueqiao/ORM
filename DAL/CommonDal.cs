using BitAuto.Utils.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
    public class CommonDal<T>
    {
        /// <summary>
        /// 主键
        /// </summary>
        private readonly Dictionary<string,SqlParameter> _primaryKeys = new Dictionary<string,SqlParameter>();

        /// <summary>
        /// 标识列
        /// </summary>
        private readonly Dictionary<string,SqlParameter> _identitis = new Dictionary<string,SqlParameter>();

        /// <summary>
        /// 表名
        /// </summary>
        private string TableName { get; set; }

        /// <summary>
        /// 标识列
        /// </summary>
        private Dictionary<string,SqlParameter> Identities
        {
            get { return _identitis; }
        }

        /// <summary>
        /// 主键字段
        /// </summary>
        private Dictionary<string,SqlParameter> PrimaryKeys
        {
            get { return _primaryKeys; }
        }

        private Dictionary<string, SqlParameter> GetSqlKeyValueDictinary(T model)
        {
            var dic = new Dictionary<string, SqlParameter>();
            Type tType = typeof (T);
            TableName = tType.Name;
            Array.ForEach(tType.GetProperties(), p =>
            {
                var result = p.GetValue(model, null);
                if (!Equals(result, null))
                {
                    var parameter = new SqlParameter(string.Format("@{0}", p.Name), p.GetValue(model, null));
                    dic.Add(p.Name, parameter);
                }
            });

            return dic;
        }


        /// <summary>
        /// 新增操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(T model)
        {
            var dic = GetSqlKeyValueDictinary(model);
            if (Equals(dic, null) || dic.Count == 0)
                return 0;

            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder(string.Format(" INSERT INTO {0} ", TableName));
            var sqlForParameters = new StringBuilder(" VALUES( ");
            int i = 0;
            foreach (var keyPairValue in dic)
            {
                sql.AppendFormat("{0}", keyPairValue.Key);
                sqlForParameters.AppendFormat("@{0}", keyPairValue.Key);
                if (i < dic.Count - 1)
                {
                    sql.Append(",");
                    sqlForParameters.AppendFormat(",");
                }

                parameters.Add(keyPairValue.Value);
                i++;
            }

            sql.Append(sqlForParameters);
#if DEBUG
            return 0;
#elif
            return SqlHelper.ExecuteNonQuery(SystemParameters.SqlConnection,CommandType.Text,sql.ToString(),parameters.ToArray());
#endif
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(T model)
        {
            throw new NotImplementedException();
        }
    }
}