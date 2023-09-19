using Lycoris.Api.EntityFrameworkCore.Shared;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Lycoris.Api.Core.EntityFrameworkCore.Impl
{
    public class SqlBuilder<T, TPrimary> where T : MySqlBaseEntity<TPrimary>
    {
        /// 实体类型
        /// </summary>
        private readonly Type _TableType;

        /// <summary>
        /// 
        /// </summary>
        private readonly string _TableName;

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get => _TableName; }

        /// <summary>
        /// ctor
        /// </summary>
        public SqlBuilder()
        {
            _TableType = typeof(T);
            var attr = _TableType.GetCustomAttribute<TableAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                _TableName = attr.Name;
            else
                _TableName = _TableType.Name;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="tableName"></param>
        public SqlBuilder(string tableName)
        {
            _TableName = tableName;
            _TableType = typeof(T);
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        /// <param name="destObj">实体</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public string Insert(T destObj, out List<MySqlParameter> param)
        {
            var sbColumn = new StringBuilder();
            var sbValues = new StringBuilder();
            param = new List<MySqlParameter>();
            sbColumn.AppendFormat("insert into {0} (", TableName);
            sbValues.AppendFormat(" values (");

            PropertyInfo[] properties = _TableType.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                var temp = ChangeType(prop.PropertyType, prop.GetValue(destObj, null));

                if (prop.Name.ToLower() == "id")
                {
                    // 如果是int或者long 则 并且值为0 则忽略
                    if ((prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) && temp?.ToString() == "0")
                        continue;
                    // 如果是string且值为空 则忽略
                    if (prop.PropertyType == typeof(string) || string.IsNullOrEmpty(temp?.ToString()))
                        continue;
                }

                sbColumn.AppendFormat("{0},", prop.Name);
                sbValues.AppendFormat("?{0},", prop.Name);

                if (temp == null)
                    param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name, DBNull.Value));
                else
                    param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name, temp));
            }

            sbColumn.Replace(",", ")", sbColumn.Length - 1, 1);
            sbValues.Replace(",", ")", sbValues.Length - 1, 1);

            return sbColumn.ToString() + sbValues.ToString();
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        /// <param name="destList">实体列表</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public string Insert(List<T> destList, out List<MySqlParameter> param)
        {
            var strAll = new StringBuilder();
            param = new List<MySqlParameter>();

            var sbColumn = new StringBuilder();
            var sbValues = new StringBuilder();

            for (int i = 0; i < destList.Count; i++)
            {
                sbColumn.AppendFormat("insert into {0} (", TableName);
                sbValues.AppendFormat(" values (");

                PropertyInfo[] properties = _TableType.GetProperties();
                foreach (PropertyInfo prop in properties)
                {
                    if (prop.GetCustomAttribute<NotMappedAttribute>() != null)
                        continue;

                    var temp = ChangeType(prop.PropertyType, prop.GetValue(destList[i], null));

                    if (prop.Name.ToLower() == "id")
                    {
                        // 如果是int或者long 则 并且值为0 则忽略
                        if ((prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)) && temp?.ToString() == "0")
                            continue;
                        // 如果是string且值为空 则忽略
                        if (prop.PropertyType == typeof(string) || string.IsNullOrEmpty(temp?.ToString()))
                            continue;
                    }

                    sbColumn.AppendFormat("{0},", prop.Name);
                    sbValues.AppendFormat("?{0}{1},", prop.Name, i);

                    if (temp == null)
                        param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name, DBNull.Value));
                    else
                        param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name + i, temp));
                }
                sbColumn.Replace(",", ")", sbColumn.Length - 1, 1);
                sbValues.Replace(",", ");", sbValues.Length - 1, 1);

                strAll.Append(sbColumn.ToString() + sbValues.ToString());

                sbColumn.Clear();
                sbValues.Clear();
            }

            return strAll.ToString();
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <param name="destObj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string Update(T destObj, out List<MySqlParameter> param)
        {
            var sbUpdateColumn = new StringBuilder();

            sbUpdateColumn.AppendFormat("Update {0} Set ", TableName);

            var sbWhere = new StringBuilder();

            param = new List<MySqlParameter>();
            var properties = _TableType.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name.ToLower() == "id")
                    sbWhere.AppendFormat(" {0}=?{0} And", prop.Name);
                else
                    sbUpdateColumn.AppendFormat("{0}=?{0},", prop.Name);

                var temp = ChangeType(prop.PropertyType, prop.GetValue(destObj, null));

                if (temp == null)
                    param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name, DBNull.Value));
                else
                    param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name, temp));
            }

            sbWhere.Replace("And", "", sbWhere.Length - 3, 3);
            sbUpdateColumn.Replace(",", "", sbUpdateColumn.Length - 1, 1);

            sbUpdateColumn.AppendFormat("\r\nWhere {0};", sbWhere.ToString());

            return sbUpdateColumn.ToString();
        }

        /// <summary>
        /// 更新语句
        /// </summary>
        /// <typeparam name="TDbParameter"></typeparam>
        /// <param name="destList"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string Update(List<T> destList, out List<MySqlParameter> param)
        {
            var strAll = new StringBuilder();
            param = new List<MySqlParameter>();

            var sbUpdateColumn = new StringBuilder();
            var sbWhere = new StringBuilder();
            var properties = _TableType.GetProperties();

            for (int i = 0; i < destList.Count; i++)
            {
                sbUpdateColumn.AppendFormat("Update {0} Set ", TableName);

                foreach (PropertyInfo prop in properties)
                {
                    if (prop.Name.ToLower() == "id")
                        sbWhere.AppendFormat(" {0}=?{0}{1} And", prop.Name, i);
                    else
                        sbUpdateColumn.AppendFormat("{0}=?{0}{1},", prop.Name, i);

                    var temp = ChangeType(prop.PropertyType, prop.GetValue(destList[i], null));

                    if (temp == null)
                        param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name + i, DBNull.Value));
                    else
                        param.Add(SqlBuilder<T, TPrimary>.CreateParameter<MySqlParameter>("?" + prop.Name + i, temp));

                }
                sbWhere.Replace("And", "", sbWhere.Length - 3, 3);
                sbUpdateColumn.Replace(",", "", sbUpdateColumn.Length - 1, 1);
                sbUpdateColumn.AppendFormat("\r\nWhere {0};", sbWhere.ToString());
                strAll.Append(sbUpdateColumn.ToString() + "\r\n");

                sbUpdateColumn.Clear();
                sbWhere.Clear();
            }

            return strAll.ToString();
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static TDbParameter CreateParameter<TDbParameter>(string paraName, object value) where TDbParameter : DbParameter
        {
            var parameter = Activator.CreateInstance<TDbParameter>();
            parameter.ParameterName = paraName;
            parameter.Value = value;
            return parameter;
        }

        /// <summary>
        /// 改变类型值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        internal object? ChangeType(Type type, object? value)
        {

            if (value == null && type.IsGenericType)
                return Activator.CreateInstance(type);

            if (value == null)
                return null;

            if (type == value.GetType())
            {
                if (type == typeof(DateTime) && (DateTime)value == DateTime.MinValue)
                    return DateTime.Now;

                return value;
            }

            if (type.IsEnum)
                return value is string ? Enum.Parse(type, (value as string)!) : Enum.ToObject(type, value);

            if (type == typeof(bool) && typeof(int).IsInstanceOfType(value))
            {
                int temp = int.Parse(value.ToString()!);
                return temp != 0;
            }

            if (!type.IsInterface && type.IsGenericType)
            {
                Type type2 = type.GetGenericArguments()[0];
                var obj = ChangeType(type2, value);
                return Activator.CreateInstance(type, new object?[] { obj });
            }

            if (value is string && type == typeof(Guid))
                return new Guid((value as string)!);

            if (value is string && type == typeof(Version))
                return new Version((value as string)!);

            if (value is not IConvertible)
                return value;

            return Convert.ChangeType(value, type);
        }
    }
}
