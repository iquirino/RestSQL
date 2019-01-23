/*
 Licensed to the Apache Software Foundation (ASF) under one
 or more contributor license agreements.  See the NOTICE file
 distributed with this work for additional information
 regarding copyright ownership.  The ASF licenses this file
 to you under the Apache License, Version 2.0 (the
 "License"); you may not use this file except in compliance
 with the License.  You may obtain a copy of the License at
     http://www.apache.org/licenses/LICENSE-2.0
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 //Ported from https://github.com/mmrath/rsql-parser
 */
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
