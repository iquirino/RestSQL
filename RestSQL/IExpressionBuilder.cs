using System.Collections.Generic;

namespace RestSQL
{
    public interface IExpressionBuilder<T>
    {
        T Or(T expression1, T expression2);

        T And(T expression1, T expression2);

        T Parenthesize(T expression);

        T Equal(string column, object value);

        T NotEqual(string column, object value);

        T LessOrEqual(string column, object value);

        T LessThan(string column, object value);

        T GreaterOrEqual(string column, object value);

        T GreaterThan(string column, object value);

        T IsNotNull(string column);

        T IsNull(string column);

        T NotLike(string column, object value);

        T Like(string column, object value);

        T NotBetween(string column, object start, object end);

        T Between(string column, object start, object end);

        T NotIn(string column, List<object> values);

        T In(string column, List<object> values);

        T Build(string query);
    }

}
