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
    public class SectionController : ControllerBase
    {
        [HttpGet("{id}", Name = "GetSection")] // Route = /api/section/2
        public ActionResult<Section> GetSection(int id)
        // takes a section id as a url parameter and returns a section object with the corresponding information 
        // the section will contain items, which contain pricelines
        {
            DbAccessManagement DAM = new DbAccessManagement();

            // get the section
            Section section = DAM.GetSection(id);

            // if it's null, then the section wasn't found
            if (section == null)
            {
                // return a 404 ERROR
                return NotFound();
            }
            else //otherwise return the section
            {
                return section;
            }
        }

        [HttpPost] // Route = /api/section
        public IActionResult CreateSection(Section section)
        // POST request that takes JSON from the request body and builds a Section object
        // returns Content Created (201) if successful, returns server error (500) if unsuccessful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.InsertSection(section);
            if (result)
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpDelete] // Route = DELETE /api/section
        // but uses the DELETE method (as opposed to the usual GET or POST
        public IActionResult DeleteSection(Section section)
        // takes a Section object from the JSON body and deletes that record
        // it only requires the id field, and ignores everything else
        // returns NotFound (404) if unsuccessful, returns NoContent (204) if successful
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.DeleteSection(section.Id);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut] // route = PUT /api/section 
        // Note that it uses PUT instead of GET or POST
        public IActionResult UpdateSection(Section section)
        {
            DbAccessManagement DAM = new DbAccessManagement();
            bool result = DAM.UpdateSection(section);
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