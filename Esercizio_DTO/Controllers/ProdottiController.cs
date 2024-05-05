using AutoMapper;
using Esercizio_DTO.Autenticazione;
using Esercizio_DTO.Database;
using Esercizio_DTO.Entities;
using Esercizio_DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Esercizio_DTO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdottiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly JwtAuthManager _jwtAuthManager;
        // private readonly IMemoryCache _memoryCache;

        public ProdottiController(AppDbContext context, IMapper mapper, JwtAuthManager jwtAuthManager) //IMemoryCache memoryCache
        {
            _context = context;
            _mapper = mapper;
            _jwtAuthManager = jwtAuthManager;
            // _memoryCache = memoryCache;
        }

        [HttpPost("Auth")]
        public IActionResult Auth(Utente credenziali)
        {
            var utente = Utenti.GetUser(credenziali.Username);
            if(utente == null || utente.Password != credenziali.Password)
            {
                return Unauthorized("Credenziali errate");
            }

            var token = _jwtAuthManager.Auth(utente.Username, utente.Password);

            if(token == null)
            {
                return Unauthorized("Utente non autorizzato");
            }

            return Ok( new { Token = token });
        }

        [HttpGet]
        public IActionResult GetProdotti()
        {
            var prodotti = _context.Prodotti.ToList();
            var prodottiDto = _mapper.Map<List<ProdottoDto>>(prodotti);
            return Ok(prodottiDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetProdotto(int id)
        {
            var prodotto = _context.Prodotti.Find(id);
            if (prodotto == null)
                return NotFound();

            return Ok(prodotto);
        }

        [HttpPost]
        [Authorize(Roles = Ruoli.ADMIN + "," + Ruoli.USER)]
        public IActionResult PostProdotti(Prodotto prodotto)
        {
            var prodottoEsistente = _context.Prodotti.Any(p => p.Id == prodotto.Id);
            if (prodottoEsistente)
                return BadRequest();

            _context.Add(prodotto);
            _context.SaveChanges();
            return CreatedAtAction("GetProdotto", new { Id = prodotto.Id }, prodotto);
        }

        [HttpPut("{id}")]
        public IActionResult PutProdotto(int id, Prodotto prodotto)
        {
            var prodottoEsistente = _context.Prodotti.Find(id);

            if(prodottoEsistente == null)
                return NotFound();

            if(id != prodotto.Id)
                return BadRequest("L'id del prodotto non può essere modificato");

            _context.Entry(prodottoEsistente).CurrentValues.SetValues(prodotto);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProdotto(int id)
        {
            var product = _context.Prodotti.Find(id);

            if (product == null)
                return NotFound();

            _context.Prodotti.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("Ricerca")]
        public IActionResult GetRicercaProdotto([FromQuery] String ricerca, [FromQuery] String proprietà)
        {
            var risultato = _context.Prodotti;
            var query = risultato.AsQueryable();
            //var risultato = _context.Prodotti.AsQueryable();                  //non scompattato da errore

            if(String.IsNullOrEmpty(ricerca) || String.IsNullOrEmpty(proprietà))
                return BadRequest();

            query = query.Where(p => p.GetType().GetProperty(proprietà).GetValue(p).ToString().ToLower().Contains(ricerca.ToLower()));

            //return Ok(risultato);
            return Ok(_mapper.Map<IEnumerable<ProdottoDto>>(risultato));
        }

        //[HttpGet("Research")]
        //public IActionResult GetProductResearch([FromQuery] string search, [FromQuery] string property)
        //{
        //    var result = _context.Prodotti.AsQueryable();
        //    if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(property))
        //        return BadRequest();
        //    // Assuming 'property' is a valid Product property name.
        //    var parameter = Expression.Parameter(typeof(Prodotto), "p");
        //    var propertyExpression = Expression.Property(parameter, property);
        //    var toStringExpression = Expression.Call(propertyExpression, "ToString", null);
        //    var toLowerExpression = Expression.Call(toStringExpression, "ToLower", null);
        //    var containsExpression = Expression.Call(toLowerExpression, "Contains", null, Expression.Constant(search.ToLower()));
        //    var lambda = Expression.Lambda<Func<Prodotto, bool>>(containsExpression, parameter);
        //    result = result.Where(lambda);
        //    return Ok(_mapper.Map<IEnumerable<ProdottoDto>>(result));
        //}
    }

}
