using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace TestDigitalSignatureServiceApi
{
    public class DigitalSignatureClientExample
    {
        string address = @"http://localhost:50740/api/";
        //string address = @"http://172.20.36.201:7077/api/";
        public DigitalSignatureClientExample(string address)
        {
            this.address = address;
        }
        public dynamic GetTemplates(string id = null, string name =null)
        {
            using (var client = new HttpClient())
            using (var response = client.GetAsync(address + $"template/get?id={id}&name={name}").Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                string result = content.ReadAsStringAsync().Result;
                dynamic templates = JToken.Parse(result);
                return templates;
            }
        }
        public void BurnFields(string path_in, string path_out, string id)
        {
            var request = new
            {
                id = id,
                path_in = path_in,
                path_out = path_out
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            using (var response = client.PostAsync(address + "pdffield/addfields", requestContent).Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
            }
        }
        public byte[] BurnFields(byte[] pdf_in, string id)
        {
            var request = new
            {
                id = id,
                bytes = pdf_in
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            return BurnFields(pdf_in, requestContent);
        }
        public byte[] BurnFields(byte[] pdf_in, List<object> pdfFields)
        {
            var request = new
            {
                fields = pdfFields,
                bytes = pdf_in
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            return BurnFields(pdf_in, requestContent);
        }
        public byte[] BurnFields(byte[] pdf_in, StringContent requestContent)
        {
            using (var client = new HttpClient())
            using (var response = client.PostAsync(address + "pdffield/addfields", requestContent).Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                var result = content.ReadAsStringAsync().Result.Trim('"');
                return Convert.FromBase64String(result);
            }
        }
        public string CreateTemplate(string name, string description, List<object> fields)
        {
            var request = new
            {
                name = name,
                description = description,
                fields = fields
            };
            var requestText = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestText, System.Text.Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            using (var response = client.PostAsync(address + "template/create", requestContent).Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                string result = content.ReadAsStringAsync().Result;
                return result;
            }
        }
        public string UpdateTemplate(string id, string name, string description, List<object> fields)
        {
            var request = new
            {
                id = id,
                name = name,
                description = description,
                fields = fields
            };
            var requestText = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestText, System.Text.Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            using (var response = client.PostAsync(address + "template/update", requestContent).Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                string result = content.ReadAsStringAsync().Result;
                return result;
            }
        }
        public dynamic SetExamplePdf(string template_id, byte[] pdfFile)
        {
            var requestContent = new ByteArrayContent(pdfFile);
            using (var client = new HttpClient())
            using (var response = client.PostAsync(address + $"template/setexamplepdf?template_id={template_id}", requestContent).Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                string result = content.ReadAsStringAsync().Result;
                dynamic templates = JToken.Parse(result);
                return templates;
            }
        }
        public byte[] GetExamplePdf(string template_id)
        {
            using (var client = new HttpClient())
            using (var response = client.GetAsync(address + $"template/getexamplepdf?template_id={template_id}").Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                var result = content.ReadAsStringAsync().Result.Trim('"');
                var bytes = Convert.FromBase64String(result);
                return bytes;
            }
        }
        public byte[] RenderPdf(string template_id, int page=1)
        {
            using (var client = new HttpClient())
            using (var response = client.GetAsync(address + $"template/renderexamplepdf?template_id={template_id}&page={page}").Result)
            using (var content = response.Content)
            {
                var result = content.ReadAsStringAsync().Result.Trim('"');
                response.EnsureSuccessStatusCode();
                var bytes = Convert.FromBase64String(result);
                return bytes;
            }
        }

        public dynamic GetCertificates(string thumbprint)
        {
            using (var client = new HttpClient())
            using (var response = client.GetAsync(address + $"certificate/list?thumbprint={thumbprint}").Result)
            using (var content = response.Content)
            {
                response.EnsureSuccessStatusCode();
                string result = content.ReadAsStringAsync().Result;
                dynamic templates = JToken.Parse(result);
                return templates;
            }
        }
        public static List<object> GetExampleList()
        {
            var list = new List<object>();
            list.Add(new
            {
                text = "test text no 1.",
                x = 150,
                y = 100,
                width = 130,
                height = 200,
                fontSize = 25,
                color = "#FF00008B",
                showborder = true,
                page = 1,
                type = "TextField"
            });
            list.Add(new
            {
                text = "test text no 2.",
                x = 210,
                y = 150,
                width = 130,
                height = 200,
                fontSize = 15,
                color = "#FF00008B",
                showborder = true,
                page = 1,
                type = "TextField"
            });
            list.Add(new
            {
                type = "SignatureField",
                name = "signature1",
                x = 100,
                y = 100,
                width = 100,
                height = 100,
                reason = "test signature 1",
                location = "locationJa",
                page = 1,
                thumbprint = "‎9e cf 30 18 0c d5 86 8a 3a 32 fb 7c a3 cf a7 9d 1a bb 5d 5d".Replace("‎", "").Replace(" ", "").ToUpper()
            });
            list.Add(new
            {
                type = "SignatureField",
                name = "signature2",
                x = 200,
                y = 200,
                width = 100,
                height = 100,
                reason = "test signature 2",
                location = "locationJa2",
                page = 1,
                thumbprint = "79 71 3c b3 43 b1 18 03 f1 41 8a 29 06 c2 f1 45 92 7e 8a d0".Replace("‎", "").Replace(" ", "").ToUpper()
            });
            return list;
        }
    }
}
