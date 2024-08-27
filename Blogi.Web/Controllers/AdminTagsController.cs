using Blogi.Web.Data;
using Blogi.Web.Models.Domain;
using Blogi.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;   

namespace Blogi.Web.Controllers
{
    
    public class AdminTagsController : Controller
    {
        private readonly BloggieDbContext bloggieDbContext;

        public AdminTagsController(BloggieDbContext bloggieDbContext) 
        {
            this.bloggieDbContext = bloggieDbContext;
        }

            
            [HttpGet]
            public IActionResult Add()
            {
                return View();
            }

            [HttpPost]

        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {


            // Mapping AddTagRequest to Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await bloggieDbContext.AddAsync(tag);
            await bloggieDbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
          
            // use dbContext to read the tags
            var tags = await bloggieDbContext.Tags.ToListAsync();

            return View(tags);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
         public async Task<IActionResult> Edit(Guid id)
        {
            //var tag= await bloggieDbContext.Tags.Find(id);
           var tag =await bloggieDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var existingTag=await bloggieDbContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                //save changes
                await bloggieDbContext.SaveChangesAsync();

                //show success notification
                return RedirectToAction("List");

            }
            //show error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
            public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
            {
                var tag = await bloggieDbContext.Tags.FindAsync(editTagRequest.Id);

                if (tag != null)
                {
                    bloggieDbContext.Tags.Remove(tag);
                    await bloggieDbContext.SaveChangesAsync();
                    // Show success notification
                    return RedirectToAction("List");
                }

                // Show an error notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
  
    }
}
