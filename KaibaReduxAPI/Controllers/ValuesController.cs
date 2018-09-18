using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KaibaReduxAPI.Models;

namespace KaibaReduxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<List<Menu>> Get()
        {
            DbAccessManagement DAM = new DbAccessManagement();
            return DAM.getMenus();



            //return new string[] { DbAccessManagement.DBTest2() };


            /*
            if(DbAccessManagement.DBTest())
            {
                return new string[] {"connected"};
            }
            else
            {
                return new string[] { "failed" };
            }
            */

            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<List<Item>> Get(int id)
        {
            Item test1 = new Item(1,"name1", "description1", 10, "picPath1");
            test1.addPriceline(new Priceline(1, "test1Price1", 5, 10));
            test1.addPriceline(new Priceline(2, "test1Price2", 10, 20));
            
            Item test2 = new Item(1, "name2", "description2", 20, "picPath2");
            test2.addPriceline(new Priceline(3, "test2Price1", 15, 10));

            Item test3 = new Item(1, "name3", "description3", 30, "picPath3");
            test3.addPriceline(new Priceline(4, "test3Price1", 5, 10));
            test3.addPriceline(new Priceline(5, "test3Price2", 10, 20));
            test3.addPriceline(new Priceline(6, "test3Price3", 50, 30));

            List<Item> list = new List<Item>();
            list.Add(test1);


            return list;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
