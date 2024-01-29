using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JSGBlazorApp.Helpers
{
    public class ConvertTableHelper
    {

        private static ConvertTableHelper _instance;
        public static ConvertTableHelper Instance => _instance ?? (_instance = new ConvertTableHelper());

        /// <summary>
        /// JSON 으로 넘어온 Data를 DataTable 형식으로 변환하여 반환.     
        /// </summary>
        /// <param name="getData"></param>
        /// <returns></returns>
        public DataTable ConvertDataTable(object getData)
        {
            List<Dictionary<string, object>> dataDic = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(getData.ToString());
            DataTable returnTable = new DataTable();

            if (dataDic.Count <= 0)
            {
                return returnTable;
            }

            for (int columnIndex = 0; columnIndex < dataDic[0].Keys.Count; columnIndex++)
            {
                string getKey = dataDic[0].Keys.ToList()[columnIndex].ToString();
                returnTable.Columns.Add(getKey);
            }

            for (int rowIndex = 0; rowIndex < dataDic.Count; rowIndex++)
            {
                DataRow returnRow = returnTable.NewRow();

                for (int columnIndex = 0; columnIndex < dataDic[rowIndex].Keys.Count; columnIndex++)
                {
                    string getKey = dataDic[rowIndex].Keys.ToList()[columnIndex].ToString();

                    object getValue;
                    dataDic[rowIndex].TryGetValue(getKey, out getValue);

                    returnRow[getKey] = Convert.ToString(getValue);
                }

                returnTable.Rows.Add(returnRow);
            }

            return returnTable;
        }
    }
}