using Microsoft.AspNetCore.Mvc;
using CarApi.Models;
using CarApi.Data;

namespace CarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrosController : ControllerBase
    {
        private readonly CarroData _carroData;

        public CarrosController(CarroData carroData)
        {
            _carroData = carroData;
        }

        // GET: api/Carros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carro>>> GetCarros()
        {
            var carros = await _carroData.GetAll();
            return Ok(carros);
        }

        // GET: api/Carros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Carro>> GetCarro(int id)
        {
            var carro = await _carroData.GetById(id);

            if (carro == null)
            {
                return NotFound();
            }

            return carro;
        }

        // POST: api/Carros
        [HttpPost]
        public async Task<ActionResult<Carro>> PostCarro(Carro carro)
        {
            var id = await _carroData.Create(carro);
            carro.Id = id;

            return CreatedAtAction(nameof(GetCarro), new { id = carro.Id }, carro);
        }

        // PUT: api/Carros/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarro(int id, [FromBody] Carro carro)
        {
            if (id != carro.Id)
            {
                return BadRequest();
            }

            var success = await _carroData.Update(carro);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Carros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarro(int id)
        {
            var success = await _carroData.Delete(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}