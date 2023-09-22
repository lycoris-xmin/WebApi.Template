using Lycoris.Api.Common;
using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Shared;
using Lycoris.Base.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.XPath;

namespace Lycoris.Api.EntityFrameworkCore.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    internal static class TableAutoBuilderExtensions
    {
        /// <summary>
        /// 自动注册表
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        public static void TableAutoBuilder(this ModelBuilder builder, Assembly assembly)
        {
            var tables = assembly.GetTypes()
                                    .Where(x => x.IsPublic && !x.IsAbstract)
                                    .Where(x => x.IsSubclassFrom(typeof(MySqlBaseEntity<>)))
                                    .Where(x => x.GetCustomAttribute<TableAttribute>(false) != null).ToList();

            if (!tables.HasValue())
                return;

            var xmlDoc = new XPathDocument(Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml"));
            var navigator = xmlDoc.CreateNavigator();

            // 添加表
            foreach (var item in tables)
            {
                // 注释
                var memberName = $"T:{item.FullName}";
                var summaryNode = navigator.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");
                var comment = summaryNode?.InnerXml.Trim();
                comment ??= "";

                var _builder = builder.Entity(item).ToTable($"{AppSettings.Sql.TablePrefix}{item.GetCustomAttribute<TableAttribute>(false)!.Name}").HasComment(comment);

                var props = item!.GetProperties()
                                 .Where(x => x.PropertyType.IsPublic)
                                 .Where(x => !x.PropertyType.IsAbstract)
                                 .Where(x => !x.PropertyType.IsGenericType)
                                 .Where(x => x.GetCustomAttribute<NotMappedAttribute>(false) == null)
                                 .ToList();

                // 属性设置
                foreach (var p in props)
                {
                    var column = p.GetCustomAttribute<TableColumnAttribute>(false) ?? new TableColumnAttribute();

                    // 主键
                    if (column.IsPrimary)
                        _builder.HasKey(p.Name);

                    _builder.Property(p.Name).TableColumnAutoBuilder(p, column, item.FullName, navigator);
                }

                // 表索引
                var tableIndexs = item.GetCustomAttributes<TableIndexAttribute>(false).ToList();
                if (tableIndexs.HasValue())
                {
                    foreach (var index in tableIndexs!)
                    {
                        if (index.IsUnique)
                            _builder.HasIndex(index.Properties).IsUnique();
                        else
                            _builder.HasIndex(index.Properties);
                    }
                }

                // 种子数据
                var instance = Activator.CreateInstance(item) as IMySqlBaseEntity;
                var data = instance!.SeedData();
                if (data.HasValue())
                    _builder.HasData(data);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="p"></param>
        /// <param name="column"></param>
        /// <param name="className"></param>
        /// <param name="navigator"></param>
        private static void TableColumnAutoBuilder(this Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder propertyBuilder, PropertyInfo? p, TableColumnAttribute column, string? className, XPathNavigator navigator)
        {
            if (p == null)
                return;

            // 主键自增
            if (column.IsIdentity)
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long))
                    propertyBuilder.ValueGeneratedOnAdd();
            }
            else if (column.IsRowVersion)
                propertyBuilder.IsRowVersion();

            // 映射数据库列类型
            if (!column.ColumnType.IsNullOrEmpty())
                propertyBuilder.HasColumnType(column.ColumnType);

            // 字符串长度限制
            if (column.StringLength > 0 && p.PropertyType == typeof(string))
                propertyBuilder.HasMaxLength(column.StringLength);

            // 必填
            if (column.Required)
                propertyBuilder.IsRequired();

            // 主键不做以下处理
            if (!column.IsPrimary)
            {
                // 默认值
                if (column.DefaultValue != null)
                {
                    if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(column.DefaultValue).ValueGeneratedNever();
                    else
                        propertyBuilder.HasDefaultValue(column.DefaultValue);
                }
                else if (column.ColumnType.IsNullOrEmpty())
                {
                    if (p.PropertyType == typeof(string) && column.StringLength > 0)
                        propertyBuilder.HasDefaultValue("");
                    else if (new Type[] { typeof(int), typeof(long), typeof(ushort), typeof(uint) }.Contains(p.PropertyType))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(bool))
                        propertyBuilder.HasDefaultValue(false).ValueGeneratedNever();
                    else if (p.PropertyType == typeof(Enum))
                        propertyBuilder.HasDefaultValue(0);
                    else if (p.PropertyType == typeof(DateTime))
                        propertyBuilder.HasDefaultValue(new DateTime(2000, 1, 1));
                }

                if (column.JsonMap)
                {
                    propertyBuilder.HasConversion(new ValueConverter<object?, string>(x => x.ToJson(), v => JsonConvert.DeserializeObject(v, p.PropertyType)));

                    if (AppSettings.Sql.Version.StartsWith("8"))
                        propertyBuilder.HasColumnType("json");
                }
                else if (column.SqlPassword)
                    propertyBuilder.HasConversion<SqlPasswrodConverter>();
                else if (column.Sensitive)
                    propertyBuilder.HasConversion<SqlSensitiveConverter>();
            }

            // 注释
            var memberName = $"P:{className}.{p.Name}";
            var summaryNode = navigator.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");
            var comment = summaryNode?.InnerXml.Trim();
            if (!comment.IsNullOrEmpty())
                propertyBuilder.HasComment(comment);
        }
    }
}
