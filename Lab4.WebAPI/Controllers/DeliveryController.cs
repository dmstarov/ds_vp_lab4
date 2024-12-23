using Lab2.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly BasketDbContext _db;

        public DeliveryController(BasketDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Delivery>> GetDeliveryesList()
        {
            var Deliveryes = _db.Deliveryes.ToList();
            return Ok(Deliveryes);
        }

        [HttpGet("{id}")]
        public ActionResult<Delivery> GetDeliveryById(int id)
        {
            var Delivery = _db.Deliveryes.FirstOrDefault(Delivery => Delivery.Id == id);
            if (Delivery == null)
            {
                return NotFound();
            }
            return Ok(Delivery);
        }

        [HttpGet("BasketId/{BasketId}")]
        public ActionResult<IEnumerable<Delivery>> GetDeliveryesByBasketId(int BasketId)
        {
            var Deliveryes = _db.Deliveryes.Where(Delivery => Delivery.BasketId == BasketId).ToList();
            if (!Deliveryes.Any())
            {
                return NotFound();
            }
            return Ok(Deliveryes);
        }

        [HttpPost]
        public IActionResult CreateDelivery([FromBody] Delivery Delivery)
        {
            if (Delivery == null)
            {
                return BadRequest("Delivery data is null");
            }

            _db.Deliveryes.Add(Delivery);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetDeliveryById), new { id = Delivery.Id }, Delivery);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDelivery(int id, [FromBody] Delivery updatedDelivery)
        {
            if (updatedDelivery == null)
            {
                return BadRequest("Delivery data is null");
            }

            var existingDelivery = _db.Deliveryes.FirstOrDefault(Delivery => Delivery.Id == id);
            if (existingDelivery == null)
            {
                return NotFound();
            }

            existingDelivery.Street = updatedDelivery.Street;
            existingDelivery.City = updatedDelivery.City;
            existingDelivery.PostalCode = updatedDelivery.PostalCode;
            existingDelivery.Country = updatedDelivery.Country;
            existingDelivery.Notes = updatedDelivery.Notes;

            _db.SaveChanges();

            return Ok(existingDelivery);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDelivery(int id)
        {
            var Delivery = _db.Deliveryes.FirstOrDefault(Delivery => Delivery.Id == id);
            if (Delivery == null)
            {
                return NotFound();
            }

            _db.Deliveryes.Remove(Delivery);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
