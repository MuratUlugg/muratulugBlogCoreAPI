using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using muratulugBlogCore.API.Models;
using muratulugBlogCore.API.Responses;

namespace muratulugBlogCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly muratulugBlogDbContext _context;

        public ArticlesController(muratulugBlogDbContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public IEnumerable<Article> GetArticle()
        {
            return _context.Article;
        }

        [HttpGet("{page}/{pageSize}")]
        public IActionResult GetArticle(int page = 1, int pageSize = 5) 
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                IQueryable<Article> query;
                query = _context.Article.Include(x => x.Category).Include(y => y.Comment).OrderByDescending(z => z.PublishDate);
                int totalCount = query.Count();
                // 5 * 1-1 = 0
                // 5 * 2-1 = 5
                var articlesResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).ToList().Select(x => new ArticleResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentMain = x.ContentMain,
                    ContentSummary = x.ContentSummary,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comment.Count,
                    Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }
                }); ;  // Gelen kaydı atla getir

                var result = new
                {
                    TotalCount = totalCount,
                    Articles = articlesResponse
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        //localhost:port/api/article/GetArticleWithCategory/12/1/5
        [HttpGet]
        [Route("GetArticleWithCategory/{id}/{page}/{pageSize}")]   // Tipe göre değil method'da göre erişim için route kullanıldı . 
        public IActionResult GetArticleWithCategory (int id ,int page=1 ,int pageSize=5)
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                IQueryable<Article> query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.CategoryId == id).OrderByDescending(x => x.PublishDate);
                var queryResult = ArticlesPagination(query, page, pageSize);
                var result = new
                {
                    TotalCount = queryResult.Item2,
                    Articles = queryResult.Item1
                };

                return Ok(result);

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //localhost:port/api/article/GetArticleWithCategory/12/1/5
        [HttpGet]
        [Route("SearchArticle/{SearchArticle}/{page}/{pageSize}")]
        public IActionResult SearchArticle(string searchText,int page=1,int pageSize = 5)
        {
            try
            {
                IQueryable<Article> query;
                query = _context.Article.Include(x => x.Category).Include(y => y.Comment).Where(z => z.Title.Contains(searchText)).OrderByDescending(f=>f.PublishDate);
                var queryResult = ArticlesPagination(query, page, pageSize);
                var result = new
                {
                    TotalCount = queryResult.Item2,
                    Articles = queryResult.Item1
                };

               return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET: api/Articles/5
        [HttpGet("{id}")]
        public IActionResult GetArticle([FromRoute] int id)
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                var article = _context.Article.Include(x => x.Category).Include(y => y.Comment).FirstOrDefault(z => z.Id == id);

                if (article == null)
                {
                    return NotFound();
                }
                ArticleResponse articleResponse = new ArticleResponse()
                {
                    Id = article.Id,
                    Title = article.Title,
                    ContentMain = article.ContentMain,
                    ContentSummary = article.ContentSummary,
                    Picture = article.Picture,
                    PublishDate = article.PublishDate,
                    ViewCount = article.ViewCount,
                    Category = new CategoryResponse() { Id = article.Category.Id, Name = article.Category.Name },
                    CommentCount = article.Comment.Count
                };

                return Ok(articleResponse);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        // PUT: api/Articles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle([FromRoute] int id, [FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != article.Id)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        [HttpPost]
        public async Task<IActionResult> PostArticle([FromBody] Article article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Article.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.Id }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Article.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
        public System.Tuple<IEnumerable<ArticleResponse>, int> ArticlesPagination(IQueryable<Article> query, int page, int pageSize)
        {
                int totalCount = query.Count();
                var articlesResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).ToList().Select(x => new ArticleResponse()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContentMain = x.ContentMain,
                    ContentSummary = x.ContentSummary,
                    Picture = x.Picture,
                    ViewCount = x.ViewCount,
                    CommentCount = x.Comment.Count,
                    Category = new CategoryResponse() { Id = x.Category.Id, Name = x.Category.Name }
                }); ;  

                return new System.Tuple<IEnumerable<ArticleResponse>, int>(articlesResponse, totalCount);
        }
    }
}