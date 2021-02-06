using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Utils
{
    /// <summary>
    /// 值-文本对
    /// </summary>
    public readonly struct ValueTextPair<TValue>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ValueTextPair(TValue value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        /// <summary>
        /// 值
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 
        /// </summary>
        public override string ToString() => $"{Value} {Text}";

    }

    public static class ValueTextPairHelper
    {
        public static IEnumerable<ValueTextPair<TValue>> ToValueTextCollection<TValue>(this Dictionary<TValue, string> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            return dictionary.Select(p=>new ValueTextPair<TValue>(p.Key, p.Value));
        }

        public static IEnumerable<ValueTextPair<TValue>> ToValueTextCollection<TValue>(this IEnumerable<KeyValuePair<TValue, string>> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            return items.Select(p=>new ValueTextPair<TValue>(p.Key, p.Value));
        }

        public static IEnumerable<KeyValuePair<TValue, string>> ToKeyValueCollection<TValue>(this IEnumerable<ValueTextPair<TValue>> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            return items.Select(p=>new KeyValuePair<TValue, string>(p.Value, p.Text));
        }
    }

}
