using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using ConsulService.Models;
using ConsulService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Coursework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly Func<IConsulClient> _consulClientFactory;
        private readonly IMessageSender _messageSender;
        private readonly DataContext _context;

        public ValuesController(Func<IConsulClient> consulClientFactory, IMessageSender messageSender, DataContext context)
        {
            _consulClientFactory = consulClientFactory;
            _messageSender = messageSender;
            _context = context;
        }
        //using (var client = _consulClientFactory())
        //{
        //    var queryResult = await client.KV.List("ConsulKV-ID-");
        //    if (queryResult.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        List<string> finalResults = new List<string>();
        //        foreach (var matchedPair in queryResult.Response)
        //        {
        //            finalResults.Add(Encoding.UTF8.GetString(matchedPair.Value, 0,
        //                matchedPair.Value.Length));
        //        }
        //        return finalResults;
        //    }
        //    return new string[0];
        //}
        [HttpGet("categories")]
        public async Task<IEnumerable<Category>> Get()
        {
            var result = _context.Categories;
            _messageSender.SendMessage($"Client received {result.Count()} categories");
            return result;
           
        }

        [HttpGet("categories/{id}")]
        public async Task<Category> Get(int id)
        {
            var result = _context.Categories.FirstOrDefault(x=>x.Id==id);
            _messageSender.SendMessage($"Client received category with id = {id}");
            return result;
        }

        [HttpPost("categories/{id}")]
        public async Task Post(int id, [FromBody]JObject jsonData)
        {
            var found = _context.Categories.FirstOrDefault(x => x.Id == Convert.ToInt32(jsonData["Id"].ToString()));
            
            if (found == null)
            {
                _context.Categories.Add(new Category() {
                    Name =jsonData["Name"].ToString(),
                    Limit =jsonData["Limit"].ToString().ToDecimal(),
                    IsCredit =jsonData["IsCredit"].ToString().ToBool()
                });
            }
            else {
                found.Name = jsonData["Name"].ToString();
                found.Limit = jsonData["Limit"].ToString().ToDecimal();
                found.IsCredit = jsonData["IsCredit"].ToString().ToBool();
            }
            
            _context.SaveChanges();
            _messageSender.SendMessage($"Client sent category with id = {id}");
        }

        [HttpDelete("categories/{id}")]
        public async Task Delete(int id)
        {
            var todelete = _context.Categories.FirstOrDefault(x=>x.Id==id);
            _context.Remove(todelete);
            _context.SaveChanges();
            _messageSender.SendMessage($"Client deleted category with id = {id}");
        }

        [HttpGet("currencies")]
        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var result = _context.Currencies;
            _messageSender.SendMessage($"Client reseived {result.Count()} currencies");
            return result;

        }

        [HttpGet("currencies/{id}")]
        public async Task<Currency> GetCurrencies(int id)
        {
            var result = _context.Currencies.FirstOrDefault(x => x.Id == id);
            _messageSender.SendMessage($"Client received category with id = {id}");
            return result;
        }

        [HttpPost("currencies/{id}")]
        public async Task PostCurrencies(int id, [FromBody]JObject jsonData)
        {
            var found = _context.Currencies.FirstOrDefault(x => x.Id == Convert.ToInt32(jsonData["Id"].ToString()));

            if (found == null)
            {
                _context.Currencies.Add(new Currency()
                {
                    Name = jsonData["Name"].ToString(),
                    Rate = jsonData["Rate"].ToString().ToDecimal(),
                });
            }
            else
            {
                found.Name = jsonData["Name"].ToString();
                found.Rate = jsonData["Rate"].ToString().ToDecimal();
            }

            _context.SaveChanges();
            _messageSender.SendMessage($"Client sent Currency with id = {id}");
        }

        [HttpDelete("currencies/{id}")]
        public async Task DeleteCurrencies(int id)
        {
            var todelete = _context.Currencies.FirstOrDefault(x => x.Id == id);
            _context.Remove(todelete);
            _context.SaveChanges();
            _messageSender.SendMessage($"Client deleted Currency with id = {id}");
        }
        [HttpGet("transactions")]
        public async Task<IEnumerable<Transaction>> GetTransactions()
        {
            var result = _context.Transactions.Include(x => x.Currency).Include(x => x.Category);
            _messageSender.SendMessage($"Client received {result.Count()} transactions");
            return result;

        }

        [HttpGet("transactions/{id}")]
        public async Task<Transaction> GetTransactions(int id)
        {
            var result = _context.Transactions.Include(x => x.Category).Include(x => x.Currency).FirstOrDefault(x => x.Id == id);
            _messageSender.SendMessage($"Client received transaction with id = {id}");
            return result;
        }

        [HttpPost("transactions/{id}")]
        public async Task PostTransactions(int id, [FromBody]JObject jsonData)
        {
            var found = _context.Transactions.FirstOrDefault(x => x.Id == Convert.ToInt32(jsonData["Id"].ToString()));

            if (found == null)
            {
                _context.Transactions.Add(new Transaction()
                {
                    Comment = jsonData["Comment"].ToString(),
                    Value = jsonData["Value"].ToString().ToDecimal(),
                    Category = _context.Categories.FirstOrDefault(x => x.Id == jsonData["CategoryId"].ToString().ToInt()),
                    Currency = _context.Currencies.FirstOrDefault(x => x.Id == jsonData["CurrencyId"].ToString().ToInt()),
                });
            }
            else
            {
                found.Comment = jsonData["Comment"].ToString();
                found.Value = jsonData["Value"].ToString().ToDecimal();
                found.Category = _context.Categories.FirstOrDefault(x => x.Id == jsonData["CategoryId"].ToString().ToInt());
                found.Currency = _context.Currencies.FirstOrDefault(x => x.Id == jsonData["CurrencyId"].ToString().ToInt());
            }

            _context.SaveChanges();
            _messageSender.SendMessage($"Client sent transaction with id = {id}");
        }

        [HttpDelete("transactions/{id}")]
        public async Task DeleteTransactions(int id)
        {
            var todelete = _context.Transactions.FirstOrDefault(x => x.Id == id);
            _context.Remove(todelete);
            _context.SaveChanges();
            _messageSender.SendMessage($"Client deleted transaction with id = {id}");
        }

        [HttpGet("rabbit")]
        public async Task<IEnumerable<LogMessage>> Rabbit()
        {
            var result = _context.Logs.ToList();
            _messageSender.SendMessage($"Client received {result.Count} logs");
            return result;
        }

        [HttpPost("login")]
        public async Task<bool> Login([FromBody]UserView user)
        {
            var old = _context.Users.FirstOrDefault(x => x.Name == user.Name);
            if (old == null) return false;
            if (old.Password != user.Password) return false;
            return true;
        }

        [HttpPost("register")]
        public async Task<bool> Register([FromBody]UserView user)
        {
            var newUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
            if (newUser == null)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else return false;
            return true;
        }

    }
    public static class MyExtWithStrings
    {
        public static int ToInt(this string str)
        {
            return Convert.ToInt32(str);
        }
        public static decimal ToDecimal(this string str)
        {
            return Convert.ToDecimal(str);
        }
        public static bool ToBool(this string str)
        {
            return Convert.ToBoolean(str);
        }
    }
}
