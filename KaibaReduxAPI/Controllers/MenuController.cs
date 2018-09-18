using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KaibaReduxAPI.Models;

namespace KaibaReduxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        // GET api/menu
        [HttpGet]
        public ActionResult<List<Menu>> GetMenus()
        // returns a list of the menus
        {
            // create DbAccessManagement object
            DbAccessManagement DAM = new DbAccessManagement();
            // return the list of menus
            System.Diagnostics.Debug.WriteLine("MenuCont: GetMenus() ");
            return DAM.getMenus();
        }

        [HttpGet("{id}", Name = "GetMenu")] // Route = /api/menu/2
        public ActionResult<Menu> GetMenu(int id)
        // takes a menu id as a url parameter and returns a menu object with the corresponding information 
        // the menu will contain sections, which contain items, which contain pricelines
        {
            DbAccessManagement DAM = new DbAccessManagement();

            // get the menu
            Menu menu = DAM.getMenu(id);

            // if it's null, then the menu wasn't found
            if (menu == null)
            {
                // return a 404 ERROR
                return NotFound();
            }
            else //otherwise return the menu
            {
                return menu;
            }
        }

        [HttpPost] // Route = /api/menu
        public IActionResult CreateMenu(Menu menu)
        // POST request that takes JSON from the request body and builds a Menu object
        // returns Content Created (201) if successful, returns server error (500) if unsuccessful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.InsertMenu(menu);
            if (result)
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpDelete] // Route = DELETE /api/menu
        // but uses the DELETE method (as opposed to the usual GET or POST
        public IActionResult DeleteMenu(Menu menu)
        // takes a menu object from the JSON body and deletes that record
        // it only requires the id field, and ignores everything else
        // returns NotFound (404) if unsuccessful, returns NoContent (204) if successful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.DeleteMenu(menu.Id);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut] // route = PUT /api/menu 
        // Note that it uses PUT instead of GET or POST
        public IActionResult UpdateMenu(Menu menu)
            // takes a menu object from the JSON body and updates that record
            // returns NoContent (204) if successful, NotFound (404) if not
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.UpdateMenu(menu);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
    }
}