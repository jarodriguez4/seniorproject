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
    public class PricelineController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetPriceline")] // Route = /api/priceline/2
        public ActionResult<Priceline> GetPriceline(int id)
        // takes a Priceline id as a url parameter and returns a Priceline object with the corresponding information 
        // the Priceline will contain pricelines
        {
            DbAccessManagement DAM = new DbAccessManagement();

            // get the Priceline
            Priceline priceline = DAM.GetPriceline(id);

            // if it's null, then the Priceline wasn't found
            if (priceline == null)
            {
                // return a 404 ERROR
                return NotFound();
            }
            else //otherwise return the Priceline
            {
                return priceline;
            }
        }

        [HttpPost] // Route = /api/priceline
        public IActionResult CreatePriceline(Priceline priceline)
        // POST request that takes JSON from the request body and builds a Priceline object
        // returns Content Created (201) if successful, returns server error (500) if unsuccessful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.InsertPriceline(priceline);
            if (result)
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpDelete] // Route = DELETE /api/priceline
        // but uses the DELETE method (as opposed to the usual GET or POST
        public IActionResult DeletePriceline(Priceline priceline)
        // takes a Priceline object from the JSON body and deletes that record
        // it only requires the id field, and ignores everything else
        // returns NotFound (404) if unsuccessful, returns NoContent (204) if successful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.DeletePriceline(priceline.Id);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut] // route = PUT /api/priceline 
        // Note that it uses PUT instead of GET or POST
        public IActionResult UpdatePriceline(Priceline priceline)
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.UpdatePriceline(priceline);
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