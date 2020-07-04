using Microsoft.AspNetCore.Mvc;
using Aurora.Models;

namespace Aurora.Api {
    [ApiController]
    [Route("[controller]/[action]")]
    public class EventController : ControllerBase {
        [HttpGet]
        public ActionResult Model(int id) {
            if (Engine.Instance.State.Models.TryGetValue(id, out var model)) {
                return Ok(new ModelData(model));
            }

            return NotFound(ModelState);
        }

        [HttpGet]
        public ActionResult Individual(int id) {
            if (Engine.Instance.State.Individuals.TryGetValue(id, out var individual)) {
                return Ok(new IndividualData(individual));
            }

            return NotFound(ModelState);
        }

        [HttpGet]
        public ActionResult Attribute(int id) {
            if (Engine.Instance.State.Attributes.TryGetValue(id, out var attr)) {
                return Ok(new AttrData(attr));
            }

            return NotFound(ModelState);
        }
    }
}
