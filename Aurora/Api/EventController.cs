using Microsoft.AspNetCore.Mvc;
using Aurora.Models;
using AuroraCore.Events;

namespace Aurora.Api {
    [ApiController]
    [Route("[controller]/[action]")]
    public class EventController : ControllerBase {
        [HttpGet]
        public ActionResult Model(int id) {
            if (Engine.Instance.State.TryGetValue(id, out IModel model)) {
                return Ok(new ModelData(model));
            }

            return NotFound(ModelState);
        }

        [HttpGet]
        public ActionResult Individual(int id) {
            if (Engine.Instance.State.TryGetValue(id, out IIndividual individual)) {
                return Ok(new IndividualData(individual));
            }

            return NotFound(ModelState);
        }

        [HttpGet]
        public ActionResult Attribute(int id) {
            if (Engine.Instance.State.TryGetValue(id, out IAttr attr)) {
                return Ok(new AttrData(attr));
            }

            return NotFound(ModelState);
        }
    }
}
