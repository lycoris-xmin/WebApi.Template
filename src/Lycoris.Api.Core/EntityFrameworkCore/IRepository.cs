using Lycoris.Api.Core.EntityFrameworkCore.Impl;
using Lycoris.Api.EntityFrameworkCore.Shared;
using MySqlConnector;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Lycoris.Api.Core.EntityFrameworkCore
{
    /// <summary>
    /// 数据库仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPrimary"></typeparam>
    public interface IRepository<T, TPrimary> where T : MySqlBaseEntity<TPrimary>
    {
        /// <summary>
        /// 所有操作取消实体跟踪
        /// 默认：取消
        /// </summary>
        bool NoTracking { get; set; }

        /// <summary>
        /// 
        /// </summary>
        SqlBuilder<T, TPrimary> SqlBuilder { get; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }

        #region GET
        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(TPrimary id);

        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetAsync(TPrimary id);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T? Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="keySelector">排序</param>
        /// <returns></returns>
        T? Get(Expression<Func<T, bool>> predicate, Expression<Func<T, TPrimary>> keySelector);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="keySelector">排序</param>
        /// <returns></returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, TPrimary>> keySelector);

        /// <summary>
        /// 取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        TResult? GetSelect<TResult>(TPrimary id, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResult?> GetSelectAsync<TResult>(TPrimary id, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        TResult? GetSelect<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的第一条数据取固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<TResult?> GetSelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// 根据条件获取数据,按数据库默认排序,取符合条件的固定列
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IQueryable<TResult> GetSelectAsIQueryable<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int limit = 1);

        /// <summary>
        /// 返回当前表的IQueryable
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// 返回当前表的IQueryable
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);


        /// <summary>
        /// 使用sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IQueryable<T> FromSql(string sql, params object[] parameters);
        #endregion

        #region Create
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>添加后的实体对象</returns>
        T Create(T data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>添加后的实体对象</returns>
        Task<T> CreateAsync(T data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回添加后的主键</returns>
        TPrimary CreateAndGetId(T data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回添加后的主键</returns>
        Task<TPrimary> CreateAndGetIdAsync(T data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        T[] Create(params T[] data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        List<T> Create(List<T> data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        Task<T[]> CreateAsync(params T[] data);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        Task<List<T>> CreateAsync(List<T> data);
        #endregion

        #region Update
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="data"></param>
        void Update(T data);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="data"></param>
        Task UpdateAsync(T data);

        /// <summary>
        /// 根据Id,更新实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        void Update(TPrimary id, Action<T> updateAction);

        /// <summary>
        /// 根据Id,更新实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        Task UpdateAsync(TPrimary id, Action<T> updateAction);

        /// <summary>
        /// 根据条件,更新实体数据
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateAction"></param>
        void Update(Func<T, bool> predicate, Action<T> updateAction);

        /// <summary>
        /// 根据条件,更新实体数据
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateAction"></param>
        Task UpdateAsync(Func<T, bool> predicate, Action<T> updateAction);

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        void UpdateFieIds(T data, [NotNull] params Expression<Func<T, object>>[] propertyExpression);

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        void UpdateFieIds(T data, [NotNull] List<Expression<Func<T, object>>> propertyExpression);

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        Task UpdateFieIdsAsync(T data, [NotNull] params Expression<Func<T, object>>[] propertyExpression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        Task UpdateFieIdsAsync(T data, [NotNull] Expression<Func<T, object>> propertyExpression);

        /// <summary>
        /// 更新指定列
        /// </summary>
        /// <param name="data"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        Task UpdateFieIdsAsync(T data, [NotNull] List<Expression<Func<T, object>>> propertyExpression);

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        void Update(params T[] data);

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        void Update(List<T> data);

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        Task UpdateAsync(params T[] data);

        /// <summary>
        /// 批量更新
        /// EFCore，当前并没有支持批量更新，更新是一条一条数据更新
        /// 若是大批量更新，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行更新
        /// </summary>
        /// <param name="data"></param>
        Task UpdateAsync(List<T> data);
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="id"></param>
        void Delete(TPrimary id);

        /// <summary>
        /// 根据主键Id删除实体
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(TPrimary id);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="data"></param>
        void Delete(T? data);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="data"></param>
        Task DeleteAsync(T? data);

        /// <summary>
        /// 批量删除
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="data"></param>
        void Delete(List<T>? data);

        /// <summary>
        /// 批量删除
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="data"></param>
        Task DeleteAsync(List<T>? data);

        /// <summary>
        /// 根据条件删除实体列表
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件删除实体列表
        /// EFCore，当前并没有支持批量删除，删除是一条一条数据删除
        /// 若是大批量删除，建议使用<see cref="ExecuteNonQuery"/>或<see cref="ExecuteNonQueryAsync"/>进行删除
        /// </summary>
        /// <param name="predicate"></param>
        Task DeleteAsync(Expression<Func<T, bool>> predicate);
        #endregion

        #region Tool
        /// <summary>
        /// 根据筛选条件判断是否存在数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据筛选条件判断是否存在数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 同步至数据库
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// 同步至数据库
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        T CreateOrUpdate(T data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<T> CreateOrUpdateAsync(T data);
        #endregion

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery([NotNull] string sql, params MySqlParameter[] parameters);

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync([NotNull] string sql, params MySqlParameter[] parameters);
    }
}
