using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types


using System.Diagnostics;

namespace m_duel_frontend.Controllers
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }
        public CustomerEntity() { }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    //[Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            // 코드 추가 부분

            // azure ml rre 시작
            // InvokeRequestResponseService().Wait();

            string result = HttpPostRequestResponseService();

            // azure ml rre 끝


            // azure table storage 시작
            //InsertEntity();
            // azure table storage 끝


            // sql database에 저장 시작
            //SqlWrite();
            // sql database에 저장 끝 

            return new string[] { result };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        public static void SqlWrite()
        {
            string myBlob = "aaa";

            using (var connection = new SqlConnection(
        "Server=tcp:dwasqldb.database.windows.net,1433;Initial Catalog=m-duel-sql;Persist Security Info=False;User ID=konan@dwasqldb;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            ))  // 연결 문자열을 복사하고 Azure SQL Database 생성시 지정한 user id와 pwd로 변경. 서버명과 DB명은 자동으로 연결 문자열에 지정
            {
                connection.Open();

                using (connection)
                {
                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO batch(blob)   VALUES(@blob)";
                    cmd.Parameters.AddWithValue("@blob", myBlob);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // -------------------------------------------- //

        public static void CreateTable()
        {
            // 저장소 연결 문자열 처리
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // table 클라이언트 개체 생성
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // 테이블 참조 설정 
            CloudTable table = tableClient.GetTableReference("people");

            // 테이블이 존재하지 않으면 생성
            table.CreateIfNotExists();
        }

        public static void InsertEntity()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Test");
            CustomerEntity customer1 = new CustomerEntity("test", "data");
            customer1.Email = "Walter@contoso.com";
            customer1.PhoneNumber = "425-555-0101";
            TableOperation insertOperation = TableOperation.Insert(customer1);
            // entity 추가 수행
            table.Execute(insertOperation);
        }

        public static void ListEntity()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("people");

            // 전체 조회
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>();

            // where 조건 조회
            //TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Harp"));
            foreach (CustomerEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }
        }

        // -------------------------------------------- //

        public static string HttpPostRequestResponseService()
        {
            var request = (HttpWebRequest)WebRequest.Create(
                //"http://requestb.in/z2ewgtz2"
                "https://asiasoutheast.services.azureml.net/workspaces/46d0e60b05b34558827abd41f11d204f/services/06fbd51255814fbcad23d02cd4607f44/execute?api-version=2.0&details=true HTTP/1.1"
                );

            var postData = "{\"Inputs\":{\"input1\":{\"ColumnNames\":[\"idx\",\"나이\",\"프로모션참여수\",\"식별자\",\"일평균게임플레이분\",\"90일내아이템구매수\",\"게임레벨범위\",\"보유크리스탈\",\"유입경로\",\"인종\",\"성별\",\"가입코드\",\"구매번호\",\"주당접속수\",\"가입국가\",\"이탈여부\"],\"Values\":[[\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"value\",\"value\",\"value\",\"0\",\"0\",\"0\",\"value\",\"value\"],[\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"0\",\"value\",\"value\",\"value\",\"0\",\"0\",\"0\",\"value\",\"value\"]]}},\"GlobalParameters\":{}}";
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] =
                "Bearer r4it0F/hzmF+V9Krh6bC3I31JDhXEzol/AE0a3yRosBpukjABxoxiojxSlKwipPgkYFoECq7mdhPhMMmo+qDgg==";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        static async Task InvokeRequestResponseService()
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                    {
                        "input1",
                        new StringTable()
                        {
                            ColumnNames = new string[] {"idx", "나이", "프로모션참여수", "식별자", "일평균게임플레이분", "90일내아이템구매수", "게임레벨범위", "보유크리스탈", "유입경로", "인종", "성별", "가입코드", "구매번호", "주당접속수", "가입국가", "이탈여부"},
                            Values = new string[,] {  { "0", "0", "0", "0", "0", "0", "0", "0", "value", "value", "value", "0", "0", "0", "value", "value" },  { "0", "0", "0", "0", "0", "0", "0", "0", "value", "value", "value", "0", "0", "0", "value", "value" },  }
                        }
                    },
                },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "r4it0F/hzmF+V9Krh6bC3I31JDhXEzol/AE0a3yRosBpukjABxoxiojxSlKwipPgkYFoECq7mdhPhMMmo+qDgg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://asiasoutheast.services.azureml.net/workspaces/46d0e60b05b34558827abd41f11d204f/services/06fbd51255814fbcad23d02cd4607f44/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }

        }
    }
}
