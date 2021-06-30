using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpenBots.Server.SDK.Model
{
    public class TableComparisonManager
    {
        public string IgnoreColumns { get; set; }

        public string LookupColumns { get; set; }


        public DataTable Differences = new DataTable();


        public static DataTable ReadDataTable(string csvPath)
        {
            return null;
            //using (var reader = new StreamReader(csvPath))
            //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //{
            //    // Do any configuration to `CsvReader` before creating CsvDataReader.
            //    using (var dr = new CsvDataReader(csv))
            //    {
            //        var dt = new DataTable();
            //        dt.Load(dr);
            //        return dt;
            //    }
            //}
        }

        public void Compare(DataTable expected, DataTable actual)
        {
            List<string> ignoreColumns = IgnoreColumns.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(s => s.Trim()).ToList();
            List<string> lookupColumns = LookupColumns.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(s => s.Trim()).ToList();

            if (lookupColumns.Contains("Result"))
                lookupColumns.Remove("Result");

            if (lookupColumns.Contains("Error"))
                lookupColumns.Remove("Error");


            InitializeDifferences(expected, lookupColumns);

            if (!AreLookupColumnsAvailableInBothTables(lookupColumns, expected, actual))
                throw new KeyNotFoundException("Lookup Columns must be present in both data tables");

            foreach (DataRow expectedRow in expected.Rows)
            {
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                StringBuilder filterExpression = new StringBuilder();
                StringBuilder sortExpression = new StringBuilder();
                foreach (string key in lookupColumns)
                {
                    if (expected.Columns.Contains(key))
                        keyValues.Add(key, expectedRow[key]);

                    if (actual.Columns.Contains(key))
                    {
                        if (!string.IsNullOrEmpty(filterExpression.ToString()))
                            filterExpression.Append(" AND ");

                        if (!string.IsNullOrEmpty(sortExpression.ToString()))
                            sortExpression.Append(" , ");

                        string keyValue = expectedRow[key].ToString().Replace("'", "");

                        filterExpression.Append($"{key} = '{keyValue}' ");
                        sortExpression.Append($"{key} ASC ");

                    }
                }
                List<DataRow> actualRows = actual.Select(filterExpression.ToString(), sortExpression.ToString()).ToList();
                if (actualRows.Count() == 0)
                {
                    AddDiff_RowNotFound(lookupColumns, expectedRow);
                    continue;
                }
                if (actualRows.Count() > 1)
                {
                    AddDiff_MultipleRowFound(lookupColumns, expectedRow);
                    continue;
                }
                DataRow actualRow = actualRows.FirstOrDefault();
                List<string> columnsMismatch = new List<string>();

                foreach (DataColumn expectedCol in expected.Columns)
                {
                    string col = expectedCol.ColumnName;
                    if (lookupColumns.Contains(col) || ignoreColumns.Contains(col))
                        continue;

                    if (!actual.Columns.Contains(col))
                    {
                        columnsMismatch.Add(col);
                        continue;
                    }

                    string expectedValue = expectedRow[col].ToString();
                    string actualValue = actualRow[col].ToString();

                    if (!expectedValue.Equals(actualValue, StringComparison.InvariantCultureIgnoreCase))
                    {
                        columnsMismatch.Add(col);
                        continue;
                    }
                }

                AddDiff_CompareResults(lookupColumns, expectedRow, columnsMismatch);
            }
        }

        private void AddDiff_CompareResults(List<string> lookupColumns, DataRow expectedRow, List<string> mismatchColumns)
        {
            DataRow DiffRow = Differences.NewRow();
            foreach (string key in lookupColumns)
            {
                DiffRow[key] = expectedRow[key];
            }
            if (mismatchColumns.Count() > 0)
            {
                DiffRow["Result"] = "Failed";
                DiffRow["Error"] = "Mismatch:" + mismatchColumns.Aggregate((i, j) => i + "," + j);
            }
            else
                DiffRow["Result"] = "Passed";

            Differences.Rows.Add(DiffRow);
        }

        private bool AreLookupColumnsAvailableInBothTables(List<string> lookupColumns, DataTable expected, DataTable actual)
        {

            List<string> expectedColumnsNotFound = new List<string>();
            List<string> actualColumnsNotFound = new List<string>();
            foreach (string key in lookupColumns)
            {
                if (!expected.Columns.Contains(key))
                    expectedColumnsNotFound.Add(key);

                if (!actual.Columns.Contains(key))
                    actualColumnsNotFound.Add(key);
            }

            if (expectedColumnsNotFound.Count() == 0 && actualColumnsNotFound.Count() == 0)
                return true;
            string expectedError = "";
            string actualError = "";

            if (expectedColumnsNotFound.Count() > 0)
                expectedError = "Expected:" + expectedColumnsNotFound.Aggregate((i, j) => i + "," + j);

            if (actualColumnsNotFound.Count() > 0)
                actualError = "Actual:" + actualColumnsNotFound.Aggregate((i, j) => i + "," + j);

            DataRow DiffRow = Differences.NewRow();
            DiffRow["Result"] = "Error";
            DiffRow["Error"] = $"Lookup Columns Not Found. {expectedError} {actualError}";
            Differences.Rows.Add(DiffRow);
            return false;
        }

        private void AddDiff_MultipleRowFound(List<string> lookupColumns, DataRow expectedRow)
        {
            DataRow DiffRow = Differences.NewRow();
            foreach (string key in lookupColumns)
            {
                DiffRow[key] = expectedRow[key];
            }
            DiffRow["Result"] = "Failed";
            DiffRow["Error"] = "Incorrect Lookup. More than one row found.";
            Differences.Rows.Add(DiffRow);

        }

        private void AddDiff_RowNotFound(List<string> keyColumns, DataRow expectedRow)
        {
            DataRow DiffRow = Differences.NewRow();
            foreach (string key in keyColumns)
            {
                DiffRow[key] = expectedRow[key];
            }
            DiffRow["Result"] = "Failed";
            DiffRow["Error"] = "Row Not Found";
            Differences.Rows.Add(DiffRow);

        }

        private void InitializeDifferences(DataTable expected, List<string> lookupColumns)
        {
            Differences.TableName = "Differences";
            foreach (string key in lookupColumns)
            {
                Type colType = expected.Columns[key].DataType;
                Differences.Columns.Add(key, colType);
            }
            Differences.Columns.Add("Error", typeof(string));
            Differences.Columns.Add("Result", typeof(string));

        }
    }
}
