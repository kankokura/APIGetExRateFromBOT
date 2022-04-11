using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DbManager;

namespace EXRate_API_BOT.Class
{
    class GetCurrency
    {
        Services.Service service = new Services.Service();
        Models.MSTER1 model = new Models.MSTER1();
        public void GetAPI()
        {
            var sysdate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
            sysdate = "2021-12-30";

            DbResultDto dbRes = service.GetCurrlist("20211117");
            if (dbRes.TotalRecords > 0)
            {
                for (int i = 0; i < dbRes.TotalRecords; i++)
                {
                    var curr = dbRes.ResultTable.Rows[i]["CURR"].ToString();

                    string http = "https://apigw1.bot.or.th/bot/public/Stat-ExchangeRate/v2/DAILY_AVG_EXG_RATE/?start_period=" + sysdate + "&end_period=" + sysdate + "&currency=" + curr + "";

                    System.Threading.Thread.Sleep(5000);
                    var client = new RestClient(http);
                    System.Threading.Thread.Sleep(5000);
                    var request = new RestRequest(Method.GET);

                    // easily add HTTP Headers
                    request.AddHeader("Accept", "application/json");
                    request.AddHeader("x-ibm-client-id", "2bc3f749-169b-4137-8f1e-4cf1596ca735");

                    // execute the request
                    System.Threading.Thread.Sleep(5000);
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;

                    if (response.StatusDescription == "Bad Request")
                    {
                        if (curr == "THB")
                        {
                            model.ERDATE = DateTime.Today.ToString("yyyyMMdd");
                            model.ERDATE = "20220104";
                            model.STYPE = "TTS";
                            model.SCURR = "THB";
                            model.SRATE = 1.00;
                            model.BTYPE = "TTB";
                            model.BCURR = "THB";
                            model.BRATE = 1.00;

                            service.InsertVal(model);
                        }

                    }
                    else
                    {
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        //dynamic item = json.Serialize<object>(content);
                        dynamic item = json.Deserialize<object>(content);

                        //dynamic item = json.Deserialize<object>("{\"result\":{\"timestamp\":\"2019 - 12 - 26 11:56:18\"}}");
                        //string test1 = item[0];
                        var result = item["result"];
                        var api = result["api"];
                        var data = result["data"];
                        var detail = data["data_detail"];

                        /* Extract Detail Object */
                        var detail_json = json.Serialize(detail);
                        dynamic detail_dy = json.Deserialize<object>(detail_json);
                        dynamic detail_ex = detail_dy[0];
                        var period = detail_ex["period"];
                        var currency_id = detail_ex["currency_id"];
                        var buying = detail_ex["buying_transfer"];
                        var selling = detail_ex["selling"];

                        model.ERDATE = DateTime.Today.ToString("yyyyMMdd");
                        model.ERDATE = "20220104";
                        model.STYPE = "TTS";
                        model.SCURR = currency_id;
                        model.SRATE = double.Parse(selling);
                        model.BTYPE = "TTB";
                        model.BCURR = currency_id;
                        model.BRATE = double.Parse(buying);

                        service.InsertVal(model);
                    }
                }
            }
        }
    }
}
