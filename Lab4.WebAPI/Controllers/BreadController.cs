using Lab2.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreadController : ControllerBase
    {
        private readonly BasketDbContext _db;

        public BreadController(BasketDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Bread>> GetBreadsList()
        {
            var Breads = _db.Breads.ToList();
            return Ok(Breads);
        }

        [HttpGet("{id}")]
        public ActionResult<Bread> GetBreadById(int id)
        {
            var Bread = _db.Breads.FirstOrDefault(Bread => Bread.Id == id);
            if (Bread == null)
            {
                return NotFound();
            }
            return Ok(Bread);
        }

        [HttpGet("BasketId/{BasketId}")]
        public ActionResult<IEnumerable<Bread>> GetBreadsByBasketId(int BasketId)
        {
            var Breads = _db.Breads.Where(Bread => Bread.BasketId == BasketId).ToList();
            if (Breads == null || !Breads.Any())
            {
                return NotFound();
            }
            return Ok(Breads);
        }

        [HttpPost]
        public IActionResult CreateBread([FromBody] Bread Bread)
        {
            if (Bread == null)
            {
                return BadRequest("Bread data is null");
            }

            _db.Breads.Add(Bread);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetBreadById), new { id = Bread.Id }, Bread);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBread(int id, [FromBody] Bread updatedBread)
        {
            if (updatedBread == null)
            {
                return BadRequest("Bread data is null");
            }

            var existingBread = _db.Breads.FirstOrDefault(Bread => Bread.Id == id);
            if (existingBread == null)
            {
                return NotFound();
            }

            existingBread.Type = updatedBread.Type;
            existingBread.Size = updatedBread.Size;
            existingBread.BasketId = updatedBread.BasketId;

            _db.SaveChanges();

            return Ok(existingBread);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBread(int id)
        {
            var Bread = _db.Breads.FirstOrDefault(Bread => Bread.Id == id);
            if (Bread == null)
            {
                return NotFound();
            }

            _db.Breads.Remove(Bread);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
