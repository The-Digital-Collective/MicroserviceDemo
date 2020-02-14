using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCWebAppWithAPIDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<Customer> customers = GetCustomer();
            return View(customers);
        }

        public ActionResult Get(int id)
        {
            Customer customer = GetCustomerByID(id);
            List<Customer> customers = new List<Customer>();
            customers.Add(customer);
            return View("Index", customer);
        }

        Customer GetCustomerByID(int id)
        {
            Customer customers = null;
            using (HttpClient client = new HttpClient())
            {
                string url = "https://localhost:44339/api/customer/" + id;
 
                //To do: move username and password into Azure Key Vault
                string input = "TestUser:@Test123";
                byte[] array = System.Text.Encoding.ASCII.GetBytes(input);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Convert.ToBase64String(array));

                Task<HttpResponseMessage> result = client.GetAsync(url);
                if (result.Result.IsSuccessStatusCode)
                {
                    Task<string> serialiseedResult = result.Result.Content.ReadAsStringAsync();
                    customers = Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(serialiseedResult.Result);
                }
            }

            return customers;
        }

        /// <summary>
        /// Calls the WebAPI and returns customer data
        /// </summary>
        /// <returns></returns>
        IEnumerable<Customer> GetCustomer()
        {
            IEnumerable<Customer> customers = null;
            using (HttpClient client = new HttpClient())
            {
                // Hardcode url to default WebAPI port for the project
                // To do: move this into AppSettings so that the AZure AppSettings value can be picked up 
                //        when the solution is deployed into it's target environment
                string url = "https://localhost:44339/api/customer";
                Uri uri = new Uri(url);

                //To do: move username and password into Azure Key Vault
                string input = "TestUser:@Test123";
                byte[] array = System.Text.Encoding.ASCII.GetBytes(input);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Convert.ToBase64String(array));

                // System.Threading.Tasks.Task<HttpResponseMessage> result = client.GetAsync(uri);
                Task<HttpResponseMessage> result = client.GetAsync(uri);
                if (result.Result.IsSuccessStatusCode)
                {
                    //System.Threading.Tasks.Task<string> response = result.Result.Content.ReadAsStringAsync();
                    Task<string> response = result.Result.Content.ReadAsStringAsync();
                    customers = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Customer>>(response.Result);
                }
            }

            return customers;
        }

    }
}