using JSGBlazorApp.Models;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSGBlazorApp.Helpers
{
    public class DatabaseViz
    {
        public static DatabaseViz Core;
        public const int deadLine = 15;
        public string Url;

        public bool CheckConnection()
        {
            return CheckDataBase(Url, deadLine);
        }

        public bool CheckDataBase(string url, int timeout = 15)
        {
            try
            {
                RestClient obj = new RestClient(url)
                {
                    Encoding = Encoding.UTF8,
                    Timeout = timeout * 1000
                };

                RestRequest request = new RestRequest("api/data/check", Method.GET);
                return (obj.Execute(request).Content ?? "").Trim('"', '\'').ToUpper() == "OK";
            }
            catch
            {
                return false;
            }
        }

     
        public async Task<DataViz> CallExecuteAsync(string SQL, object parameters)
        {
            DataViz result = null;
            await Task.Run(() => result = CallExecute(SQL, parameters));
            return result;
        }

        public DataViz CallExecute(string SQL, object parameters = null)
        {
            try
            {
                var jProp = JObject.FromObject(parameters);

                var api = new RestClient(Url);
                api.Encoding = Encoding.UTF8;
                api.Timeout = deadLine * 1000;

                var callRequest = new RestRequest("api/data/CallExecute", Method.POST);
                callRequest.AddHeader("Content-Type", "application/json");
                callRequest.AddQueryParameter("SQL", SQL);
                callRequest.AddQueryParameter("parameters", jProp.ToString());

                var result = api.Execute(callRequest);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (result.ErrorException != null)
                    {
                        throw result.ErrorException;
                    }
                    else
                    {
                        throw new Exception(result.Content);
                    }
                }

                var deserializeObject = JsonConvert.DeserializeObject<DataViz>(result.Content);
                return deserializeObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataViz> CallQueryAsync(string SQL, object parameters = null)
        {
            DataViz result = null;
            await Task.Run(() => result = CallQuery(SQL, parameters));
            return result;
        }

        public DataViz CallQuery(string SQL, object parameters = null)
        {
            try
            {
                JObject jProp;

                if (parameters != null)
                {
                    jProp = JObject.FromObject(parameters);
                }
                else
                {
                    jProp = new JObject();
                }

                var api = new RestClient(Url);
                api.Encoding = Encoding.UTF8;
                api.Timeout = deadLine * 1000;

                var callRequest = new RestRequest("api/data/CallQuery", Method.POST);
                callRequest.AddHeader("Content-Type", "application/json");
                callRequest.AddQueryParameter("SQL", SQL);
                callRequest.AddQueryParameter("parameters", jProp.ToString());

                var result = api.Execute(callRequest);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (result.ErrorException != null)
                    {
                        throw result.ErrorException;
                    }
                    else
                    {
                        throw new Exception(result.Content);
                    }
                }

                var deserializeObject = JsonConvert.DeserializeObject<DataViz>(result.Content);
                return deserializeObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataViz> CallExecuteAsyncMP(string SQL, object parameters)
        {
            DataViz result = null;
            await Task.Run(() => result = CallExecuteMP(SQL, parameters));
            return result;
        }

        /// <summary>
        /// MessagePack Post 
        /// JSON 길이가 너무 커서 API에 Request가 안될 경우를 대비해 MessagePack으로 데이터를 압축하여 보내준다.
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataViz CallExecuteMP(string SQL, object parameters = null)
        {
            try
            {
                var jProp     = JObject.FromObject(parameters);
                var jsonParam = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jProp.ToString());

                Dictionary<string, Dictionary<string, dynamic>> Data = new Dictionary<string, Dictionary<string, dynamic>>();
                Data.Add(SQL, jsonParam);

                DataParamModel model = new DataParamModel { Data = Data };
                byte[] serializedData = MessagePackSerializer.Serialize(model);

                var api = new RestClient(Url);
                api.Encoding = Encoding.UTF8;
                api.Timeout = deadLine * 1000;

                var callRequest = new RestRequest("api/data/CallExecuteMP", Method.POST);
                callRequest.AddHeader("Content-Type", "application/json");
                callRequest.AddJsonBody(serializedData);

                var result = api.Execute(callRequest);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (result.ErrorException != null)
                    {
                        throw result.ErrorException;
                    }
                    else
                    {
                        throw new Exception(result.Content);
                    }
                }

                var deserializeObject = JsonConvert.DeserializeObject<DataViz>(result.Content);
                return deserializeObject;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Dictionary<string, object>> GetDataViz(DataViz jsonData, int index)
        {
            try
            {
                var getReuslt = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData.Table[index]);
                return getReuslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
