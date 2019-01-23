using System;
using System.Collections.Generic;
using System.Text;

namespace RestSQL.SqlKata
{
    public interface IColumnNameTransform
    {
        string Transform(string columnName);
    }
}
