using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieList.Models;

namespace MovieList.Controllers
{
   // [ApiController]
  // [Route("movies/getall")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Movie Movie { get; set; }
        public MoviesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
          // IEnumerable<MoviesController> objList = _db.Movies;
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Movie = new Movie();
            if (id == null)
            {
                //create
                return View(Movie);
            }
            //update
            Movie = _db.Movies.FirstOrDefault(u => u.Id == id);
            if (Movie == null)
            {
                return NotFound();
            }

            return View(Movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Movie.Id == 0)
                {
                    //create
                    _db.Movies.Add(Movie);
                }
                else
                {
                    _db.Movies.Update(Movie);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Movie);
        }


        #region API calls


        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Movies.ToListAsync() });
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(int id)
        {
            var movieFromDb = await _db.Movies.FirstOrDefaultAsync(u => u.Id == id);
            if (movieFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _db.Movies.Remove(movieFromDb);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion

    }
   
}
