using Lab2.DataAccess;
using Lab4.WebAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab4.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly BasketDbContext _db;

        public BasketController(BasketDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BasketDto>> GetBasketsList()
        {
            var Baskets = _db.Baskets
                .Include(Basket => Basket.Deliveryes)
                .Include(Basket => Basket.Breads)
                .Select(Basket => new BasketDto
                {
                    Id = Basket.Id,
                    YearBuilt = Basket.YearBuilt,
                    Owner = Basket.Owner,
                    Area = Basket.Area,
                    Floors = Basket.Floors,
                    Deliveryes = Basket.Deliveryes.Select(Delivery => new DeliveryDto
                    {
                        Street = Delivery.Street,
                        City = Delivery.City,
                        PostalCode = Delivery.PostalCode,
                        Country = Delivery.Country,
                        Notes = Delivery.Notes
                    }).ToList(),
                    Breads = Basket.Breads.Select(Bread => new BreadDto
                    {
                        Type = Bread.Type,
                        Size = Bread.Size
                    }).ToList()
                })
                .ToList();

            return Ok(Baskets);
        }

        [HttpGet("{id}")]
        public ActionResult<BasketDto> GetBasketById(int id)
        {
            var Basket = _db.Baskets
                .Include(Basket => Basket.Deliveryes)
                .Include(Basket => Basket.Breads)
                .FirstOrDefault(Basket => Basket.Id == id);

            if (Basket == null)
            {
                return NotFound();
            }

            var BasketDto = new BasketDto
            {
                Id = Basket.Id,
                YearBuilt = Basket.YearBuilt,
                Owner = Basket.Owner,
                Area = Basket.Area,
                Floors = Basket.Floors,
                Deliveryes = Basket.Deliveryes.Select(Delivery => new DeliveryDto
                {
                    Street = Delivery.Street,
                    City = Delivery.City,
                    PostalCode = Delivery.PostalCode,
                    Country = Delivery.Country,
                    Notes = Delivery.Notes
                }).ToList(),
                Breads = Basket.Breads.Select(Bread => new BreadDto
                {
                    Type = Bread.Type,
                    Size = Bread.Size
                }).ToList()
            };

            return Ok(BasketDto);
        }

        [HttpPost]
        public IActionResult CreateBasket([FromBody] BasketDto BasketDto)
        {
            if (BasketDto == null)
            {
                return BadRequest("Basket data is null.");
            }

            var Basket = new Basket
            {
                YearBuilt = BasketDto.YearBuilt,
                Owner = BasketDto.Owner,
                Area = BasketDto.Area,
                Floors = BasketDto.Floors
            };

            if (BasketDto.Deliveryes != null && BasketDto.Deliveryes.Count > 0)
            {
                Basket.Deliveryes = BasketDto.Deliveryes.Select(DeliveryDto => new Delivery
                {
                    Street = DeliveryDto.Street,
                    City = DeliveryDto.City,
                    PostalCode = DeliveryDto.PostalCode,
                    Country = DeliveryDto.Country,
                    Notes = DeliveryDto.Notes
                }).ToList();
            }

            if (BasketDto.Breads != null && BasketDto.Breads.Count > 0)
            {
                Basket.Breads = BasketDto.Breads.Select(BreadDto => new Bread
                {
                    Type = BreadDto.Type,
                    Size = BreadDto.Size
                }).ToList();
            }

            _db.Baskets.Add(Basket);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetBasketById), new { id = Basket.Id }, BasketDto);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateBasket(int id, [FromBody] BasketDto BasketDto)
        {
            var existingBasket = _db.Baskets
                .Include(Basket => Basket.Deliveryes)
                .Include(Basket => Basket.Breads)
                .FirstOrDefault(Basket => Basket.Id == id);

            if (existingBasket == null)
            {
                return NotFound($"Basket with ID {id} not found.");
            }

            existingBasket.Owner = BasketDto.Owner;
            existingBasket.YearBuilt = BasketDto.YearBuilt;
            existingBasket.Area = BasketDto.Area;
            existingBasket.Floors = BasketDto.Floors;

            if (BasketDto.Deliveryes != null)
            {
                _db.Deliveryes.RemoveRange(existingBasket.Deliveryes);

                existingBasket.Deliveryes = BasketDto.Deliveryes.Select(DeliveryDto => new Delivery
                {
                    Street = DeliveryDto.Street,
                    City = DeliveryDto.City,
                    PostalCode = DeliveryDto.PostalCode,
                    Country = DeliveryDto.Country,
                    Notes = DeliveryDto.Notes
                }).ToList();
            }

            if (BasketDto.Breads != null)
            {
                _db.Breads.RemoveRange(existingBasket.Breads);

                existingBasket.Breads = BasketDto.Breads.Select(BreadDto => new Bread
                {
                    Type = BreadDto.Type,
                    Size = BreadDto.Size
                }).ToList();
            }

            _db.SaveChanges();

            var updatedBasketDto = new BasketDto
            {
                Id = existingBasket.Id,
                YearBuilt = existingBasket.YearBuilt,
                Owner = existingBasket.Owner,
                Area = existingBasket.Area,
                Floors = existingBasket.Floors,
                Deliveryes = existingBasket.Deliveryes.Select(Delivery => new DeliveryDto
                {
                    Street = Delivery.Street,
                    City = Delivery.City,
                    PostalCode = Delivery.PostalCode,
                    Country = Delivery.Country,
                    Notes = Delivery.Notes
                }).ToList(),
                Breads = existingBasket.Breads.Select(Bread => new BreadDto
                {
                    Type = Bread.Type,
                    Size = Bread.Size
                }).ToList()
            };

            return Ok(updatedBasketDto);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBasket(int id)
        {
            var Basket = _db.Baskets.FirstOrDefault(Basket => Basket.Id == id);
            if (Basket == null)
            {
                return NotFound();
            }

            _db.Baskets.Remove(Basket);
            _db.SaveChanges();
            return NoContent();
        }
    }
}
