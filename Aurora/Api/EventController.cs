using Microsoft.AspNetCore.Mvc;
using Aurora.Models;
using System.Threading.Tasks;

namespace Aurora.Api {
    [ApiController]
    [Route("[controller]/[action]")]
    public class EventController : ControllerBase {
        [HttpGet]
        public async Task<ActionResult> Model(int id) {
            var model = await Engine.Instance.Storage.GetModel(id);
            if (null == model) {
                return NotFound(ModelState);
            }
            else {
                return Ok(await ModelData.Instantiate(model));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Individual(int id) {
            var individual = await Engine.Instance.Storage.GetIndividual(id);
            if (null == individual) {
                return NotFound(ModelState);
            }
            else {
                return Ok(await IndividualData.Instantiate(individual));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Attribute(int id) {
            var attribute = await Engine.Instance.Storage.GetAttribute(id);
            if (null == attribute) {
                return NotFound(ModelState);
            }
            else {
                return Ok(await AttrData.Instantiate(attribute));
            }
        }
    }
}
