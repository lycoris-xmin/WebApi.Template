using Lycoris.Api.Common;
using Lycoris.Api.EntityFrameworkCore.Contexts;
using Lycoris.Api.EntityFrameworkCore.Shared;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySqlConnector;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Lycoris.Api.Core.EntityFrameworkCore.Impl
{
    /// <summary>
    /// 数据库仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class Repository<T, TPrimary> : IRepository<T, TPrimary> where T : MySqlBaseEntity<TPrimary>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly MySqlContext _context;

        private SqlBuilder<T, TPrimary>? _SqlBuilder = null;

        /// <summary>
        /// 
        /// </summary>
        public SqlBuilder<T, TPrimary> SqlBuilder
        {
            get
            {
                if (_SqlBuilder != null)
                    return _SqlBuilder;

                _SqlBuilder ??= new SqlBuilder<T, TPrimary>(TableName);
                return _SqlBuilder;
            }
        }

        private string? _TableName = null;
        /// <summary>
        /// 表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_TableName))
                    return _TableName;

                var attr = typeof(T).GetCustomAttribute<TableAttribute>();
                if (attr != null && !string.IsNullOrEmpty(attr.Name))
                    _TableName = attr.Name;
                else
                    _TableName = typeof(T).Name;

                _TableName = $"{AppSettings.Sql.TablePrefix}{_TableName}";

                return _TableName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NoTracking { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public Repository(MySqlContext context)
        {
            _context = context;

            // 加快第一次加载数据库的速度
            if (AppSettings.IsDebugger)
                context.Database.EnsureCreated();
        }

        #region Get
        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T? Get(TPrimary id) => _context.Set<T>().AsNoTrackingIf(NoTracking).Where(x => x.Id!.Equals(id)).SingleOrDefault();

        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T?> GetAsync(TPrimary id) => await _context.Set<T>().AsNoTrackingIf(NoTracking).SingleOrDefaultAsync(x => x.Id!.Equals(id));

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T? Get(Expression<Func<T, bool>> predicate) => _context.Set<T>().AsNoTrackingIf(NoTracking).FirstOrDefault(predicate);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AsNoTrackingIf(NoTracking).FirstOrDefaultAsync(predicate);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="keySelector">排序</param>
        /// <returns></returns>
        public virtual T? Get(Expression<Func<T, bool>> predicate, Expression<Func<T, TPrimary>> keySelector) => _context.Set<T>().AsNoTrackingIf(NoTracking).OrderBy(keySelector).FirstOrDefault(predicate);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="keySelector">排序</param>
        /// <returns></returns>
        public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, TPrimary>> keySelector) => await _context.Set<T>().AsNoTrackingIf(NoTracking).OrderBy(keySelector).FirstOrDefaultAsync(predicate);

        /// <summary>
        /// 取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual TResult? GetSelect<TResult>(TPrimary id, Expression<Func<T, TResult>> selector)
               => _context.Set<T>().AsNoTrackingIf(NoTracking).Where(x => x.Id!.Equals(id)).Select(selector).SingleOrDefault();

        /// <summary>
        /// 取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<TResult?> GetSelectAsync<TResult>(TPrimary id, Expression<Func<T, TResult>> selector)
               => await _context.Set<T>().AsNoTrackingIf(NoTracking).Where(x => x.Id!.Equals(id)).Select(selector).SingleOrDefaultAsync();

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual TResult? GetSelect<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
               => _context.Set<T>().AsNoTrackingIf(NoTracking).Where(predicate).Select(selector).FirstOrDefault();

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<TResult?> GetSelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
               => await _context.Set<T>().AsNoTrackingIf(NoTracking).Where(predicate).Select(selector).FirstOrDefaultAsync();

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public virtual IQueryable<TResult> GetSelectAsIQueryable<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int limit = 1)
            => _context.Set<T>().AsNoTrackingIf(NoTracking).Where(predicate).OrderBy(x => x.Id).Take(limit).Select(selector);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll() => _context.Set<T>().AsNoTracking();

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate) => _context.Set<T>().AsNoTrackingIf(NoTracking).Where(predicate);

        /// <summary>
        /// 使用sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IQueryable<T> FromSql([NotNull] string sql, params object[] parameters) => _context.Set<T>().FromSqlRaw(sql, parameters);
        #endregion

        #region Create
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual T Create(T data)
        {
            var db = _context.Set<T>().Add(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
            return db.Entity;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<T> CreateAsync(T data)
        {
            var db = await _context.Set<T>().AddAsync(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
            return db.Entity;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual TPrimary CreateAndGetId(T data)
        {
            var db = _context.Set<T>().Add(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
            return db.Entity.Id;
        }

        /// <summary>
        /// 添加数据 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<TPrimary> CreateAndGetIdAsync(T data)
        {
            var db = await _context.Set<T>().AddAsync(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
            return db.Entity.Id;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual T[] Create(params T[] data)
        {
            _context.Set<T>().AddRange(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
            return data;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual List<T> Create(List<T> data)
        {
            _context.Set<T>().AddRange(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
            return data;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<T[]> CreateAsync(params T[] data)
        {
            _context.Set<T>().AddRange(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
            return data;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> CreateAsync(List<T> data)
        {
            _context.Set<T>().AddRange(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
            return data;
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void Update(T data)
        {
            _context.Set<T>().Update(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void Update(TPrimary id, Action<T> updateAction)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            updateAction(entity);

            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 根据Id,更新实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        public virtual async Task UpdateAsync(T data)
        {
            _context.Set<T>().Update(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 根据Id,更新实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        public virtual async Task UpdateAsync(TPrimary id, Action<T> updateAction)
        {
            var entity = await GetAsync(id);
            if (entity == null)
                return;

            updateAction(entity);

            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 根据条件,更新实体数据
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateAction"></param>
        public virtual void Update(Func<T, bool> predicate, Action<T> updateAction)
        {
            var entity = _context.Set<T>().Where(predicate).ToList();

            if (entity == null || !entity.Any())
                return;

            entity.ForEach(x => updateAction(x));
            _context.Set<T>().UpdateRange(entity);
            _context.SaveChanges();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 根据条件,更新实体数据
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateAction"></param>
        public virtual async Task UpdateAsync(Func<T, bool> predicate, Action<T> updateAction)
        {
            var entity = _context.Set<T>().Where(predicate).ToList();

            if (entity == null || !entity.Any())
                return;

            entity.ForEach(x => updateAction(x));
            _context.Set<T>().UpdateRange(entity);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public virtual void UpdateFieIds(T data, [NotNull] params Expression<Func<T, object>>[] propertyExpression)
        {
            if (propertyExpression.Length == 0)
                return;

            _context.Set<T>().Attach(data);

            foreach (var item in propertyExpression)
            {
                _context.Entry(data).Property(item).IsModified = true;
            }

            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public virtual void UpdateFieIds(T data, [NotNull] List<Expression<Func<T, object>>> propertyExpression)
        {
            if (!propertyExpression.Any())
                return;

            _context.Set<T>().Attach(data);

            foreach (var item in propertyExpression)
            {
                _context.Entry(data).Property(item).IsModified = true;
            }

            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public virtual async Task UpdateFieIdsAsync(T data, [NotNull] params Expression<Func<T, object>>[] propertyExpression)
        {
            if (propertyExpression.Length == 0)
                return;

            _context.Set<T>().Attach(data);

            foreach (var item in propertyExpression)
            {
                _context.Entry(data).Property(item).IsModified = true;
            }

            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public virtual async Task UpdateFieIdsAsync(T data, [NotNull] Expression<Func<T, object>> propertyExpression)
        {
            _context.Set<T>().Attach(data);

            foreach (var item in propertyExpression.GetMemberAccessList())
            {
                _context.Entry(data).Property(item.Name).IsModified = true;
            }

            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }


        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public virtual async Task UpdateFieIdsAsync(T data, [NotNull] List<Expression<Func<T, object>>> propertyExpression)
        {
            if (!propertyExpression.Any())
                return;

            _context.Set<T>().Attach(data);

            foreach (var item in propertyExpression)
            {
                _context.Entry(data).Property(item).IsModified = true;
            }

            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        public virtual void Update([NotNull] params T[] data)
        {
            if (data.Any(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to update contains an nullable object");

            _context.Set<T>().UpdateRange(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        public virtual void Update([NotNull] List<T> data)
        {
            if (data.All(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to update contains an nullable object");

            _context.Set<T>().UpdateRange(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        public virtual async Task UpdateAsync([NotNull] params T[] data)
        {
            if (data.Any(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to update contains an nullable object");

            _context.Set<T>().UpdateRange(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        public virtual async Task UpdateAsync([NotNull] List<T> data)
        {
            if (data.Any(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to update contains an nullable object");

            _context.Set<T>().UpdateRange(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(TPrimary id)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            _context.Remove(entity);
            _context.SaveChanges();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task DeleteAsync(TPrimary id)
        {
            var entity = await GetAsync(id);

            if (entity == null)
                return;

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="data"></param>
        public virtual void Delete(T? data)
        {
            if (data == null)
                return;

            _context.Remove(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="data"></param>
        public virtual async Task DeleteAsync(T? data)
        {
            if (data == null)
                return;

            _context.Remove(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量删除
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="data"></param>
        public virtual void Delete(List<T>? data)
        {
            if (data == null)
                return;

            if (data.All(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to delete contains an nullable object");

            _context.RemoveRange(data);
            _context.SaveChanges();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 批量删除
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="data"></param>
        public virtual async Task DeleteAsync(List<T>? data)
        {
            if (data == null)
                return;

            if (data.All(x => x == null))
                throw new ArgumentNullException(nameof(data), "the list to delete contains an nullable object");

            _context.RemoveRange(data);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(data);
        }

        /// <summary>
        /// 根据条件删除实体列表
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var entity = GetAll(predicate).ToList();
            if (entity == null || !entity.Any())
                return;

            _context.RemoveRange(entity);
            _context.SaveChanges();
            ChangeDetachedTracking(entity);
        }

        /// <summary>
        /// 根据条件删除实体列表
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await GetAll(predicate).ToListAsync();
            if (entity == null || !entity.Any())
                return;

            _context.RemoveRange(entity);
            await _context.SaveChangesAsync();
            ChangeDetachedTracking(entity);
        }
        #endregion

        #region Tool
        /// <summary>
        /// 根据筛选条件判断是否存在数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Exists(Expression<Func<T, bool>> predicate) => _context.Set<T>().Any(predicate);

        /// <summary>
        /// 根据筛选条件判断是否存在数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().AnyAsync(predicate);

        /// <summary>
        /// 同步至数据库
        /// </summary>
        /// <returns></returns>
        public int SaveChanges() => _context.SaveChanges();

        /// <summary>
        /// 同步至数据库
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public T CreateOrUpdate(T data)
        {
            var type = typeof(TPrimary);
            if ((type == typeof(int) || type == typeof(long)) && (data.Id == null || data.Id.ToString() == "0"))
                return Create(data);
            else if ((type == typeof(string) || type == typeof(Guid)) && (data.Id == null || data.Id.ToString().IsNullOrEmpty()))
                return Create(data);

            Update(data);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<T> CreateOrUpdateAsync(T data)
        {
            var type = typeof(TPrimary);
            if ((type == typeof(int) || type == typeof(long)) && (data.Id == null || data.Id.ToString() == "0"))
                return await CreateAsync(data);
            else if ((type == typeof(string) || type == typeof(Guid)) && (data.Id == null || data.Id.ToString().IsNullOrEmpty()))
                return await CreateAsync(data);

            await UpdateAsync(data);
            return data;
        }
        #endregion

        #region SQL语句
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery([NotNull] string sql, params MySqlParameter[] parameters) => _context.Database.ExecuteSqlRaw(sql, parameters);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteNonQueryAsync([NotNull] string sql, params MySqlParameter[] parameters) => await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        #endregion

        /// <summary>
        /// 移除实体跟踪状态
        /// </summary>
        /// <param name="data"></param>
        private void ChangeDetachedTracking(T data)
        {
            if (!NoTracking)
                return;

            if (_context.Entry(data).State != EntityState.Detached)
                _context.Entry(data).State = EntityState.Detached;
        }

        /// <summary>
        /// 移除实体跟踪状态
        /// </summary>
        /// <param name="data"></param>
        private void ChangeDetachedTracking(params T[] data)
        {
            if (!NoTracking || data == null || data.Length == 0)
                return;

            // 为避免影响效率，批量处理超过100个的不再操作实体跟踪操作
            if (data.Length > 100)
                return;

            foreach (var item in data)
            {
                if (_context.Entry(item).State != EntityState.Detached)
                    _context.Entry(item).State = EntityState.Detached;
            }
        }

        /// <summary>
        /// 移除实体跟踪状态
        /// </summary>
        /// <param name="data"></param>
        private void ChangeDetachedTracking(List<T>? data)
        {
            if (!NoTracking || data == null || !data.Any())
                return;

            // 为避免影响效率，批量处理超过100个的不再操作实体跟踪操作
            if (data.Count > 100)
                return;

            foreach (var item in data)
            {
                if (_context.Entry(item).State != EntityState.Detached)
                    _context.Entry(item).State = EntityState.Detached;
            }
        }
    }

    internal static class RepositoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal static IQueryable<T> AsNoTrackingIf<T>(this IQueryable<T> query, bool condition) where T : class
            => condition ? query.AsNoTracking() : query;
    }
}
