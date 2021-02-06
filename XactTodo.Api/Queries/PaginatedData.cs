using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Queries
{
    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedData<T>
    {
        /// <summary>
        /// 第几页，从1开始
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// 本次返回的数据行
        /// </summary>
        public IEnumerable<T> Rows { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">数据行</param>
        /// <param name="count">记录总数</param>
        /// <param name="pageIndex">第几页，从1开始</param>
        /// <param name="pageSize">每页记录数</param>
        public PaginatedData(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            this.Total = count;
            this.Rows = items;
            PageIndex = pageIndex;
            TotalPages = count > 0 && pageSize < 1 ? 1 : (int)Math.Ceiling(count / (double)pageSize);
        }

        /// <summary>
        /// 是否有上页
        /// </summary>
        public bool HasPreviousPage => (PageIndex > 1);

        /// <summary>
        /// 是否有下页
        /// </summary>
        public bool HasNextPage => (PageIndex < TotalPages);

        /// <summary>
        /// 执行查询并返回指定页的数据
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="pageIndex">第几页，从1开始</param>
        /// <param name="pageSize">每页数据行</param>
        /// <returns></returns>
        public static async Task<PaginatedData<T>> ExecuteAsync(IQueryable<T> query, int pageIndex, int pageSize)
        {
            var count = await query.CountAsync();
            IEnumerable<T> items;
            if (pageSize < 1 || count == 0) //不分页
            {
                pageIndex = 1;
                pageSize = count;
                items = await query.ToArrayAsync();
            }
            else
            {
                items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToArrayAsync();
            }
            return new PaginatedData<T>(items, count, pageIndex < 1 ? 1 : pageIndex, pageSize);
        }

        /// <summary>
        /// 异步执行分页查询
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="selectScript">SELECT脚本，可包含<code>WHERE</code>但不能包含<code>ORDER BY子句</code></param>
        /// <param name="param">查询参数对象</param>
        /// <param name="orderBy">排序字段，格式如：column1[, column2 [DESC]]</param>
        /// <param name="pageIndex">第几页，从1开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns></returns>
        public static async Task<PaginatedData<T>> ExecuteAsync(IDbConnection connection, string selectScript, object param, string orderBy, int pageIndex, int pageSize, IDbTransaction transaction = null)
        {
            const string ORDER_BY = "ORDER BY";
            var count = await connection.ExecuteScalarAsync<int>($"SELECT COUNT(1) FROM ({selectScript}) T", param, transaction);
            if (count == 0)
            {
                return new PaginatedData<T>(new T[] { }, count, 1, pageSize);
            }
            if (pageSize <= 0) //不分页
            {
                var data = await connection.QueryAsync<T>(selectScript, param, transaction);
                return new PaginatedData<T>(data, count, 1, pageSize);
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                //如果未指定排序字段，则对第1个字段排序后分页
                using (var reader = connection.ExecuteReader($"SELECT * FROM ({selectScript}) T WHERE 1>2", param, transaction))
                {
                    var firstColumnName = reader.GetName(0);
                    orderBy = $"{ORDER_BY} {firstColumnName}";
                }
            }
            else
            {
                orderBy = orderBy.Trim();
                if (!orderBy.StartsWith(ORDER_BY, StringComparison.InvariantCultureIgnoreCase))
                    orderBy = $"{ORDER_BY} {orderBy}";
            }
            if (pageIndex < 1) pageIndex = 1;
            var connectionTypeName = connection.GetType().Name;
            //MySQL
            if (connectionTypeName.Contains("MySqlConnection", StringComparison.InvariantCultureIgnoreCase))
            {
                var sqlPage = $"{selectScript} {orderBy} LIMIT {(pageIndex - 1) * pageSize}, {pageSize}";
                var rows = await connection.QueryAsync<T>(sqlPage, param, transaction);
                return new PaginatedData<T>(rows, count, pageIndex, pageSize);
            }
            //SQL SERVER
            else if (connectionTypeName.Contains("SqlConnection", StringComparison.InvariantCultureIgnoreCase))
            {
                var select = $"SELECT *, [row_number]=ROW_NUMBER() OVER ({orderBy}) FROM (\n{selectScript}\n) T";
                var sqlPage = $"SELECT * FROM(\n{select}\n) A WHERE A.[row_number] BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}";
                //sqlPage += Environment.NewLine + orderBy;
                var rows = await connection.QueryAsync<T>(sqlPage, param, transaction);
                return new PaginatedData<T>(rows, count, pageIndex, pageSize);
            }
            else
            {
                throw new NotSupportedException("不支持的数据库类型");
            }
        }
    }
}
