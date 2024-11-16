using Microsoft.AspNetCore.Mvc;
using MobileProject.Repository;
namespace MobileProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMbtileRepository _mbtileRepository;
        public MapController(IMbtileRepository mbtileRepository)
        {
            _mbtileRepository = mbtileRepository;
        }
        [HttpGet("tiles/{z}/{x}/{y}.pbf")]
        public IActionResult GetMapTile(int z=5,int x=31,int y=31)
        {
            int mod_y = (1 << z) - 1 - y;
            var data=_mbtileRepository.ReadTile(x,mod_y,z);
            Response.Headers.ContentEncoding = "gzip";
            return File(data,"application/octet-stream",$"{z}/{x}/{y}.pbf");
        }
        [HttpGet("style")]
        public IActionResult GetMapStyle()
        {
            string filePath = "./tileset.json";
            if(System.IO.File.Exists(filePath))
            {
                var fileContent = System.IO.File.ReadAllText(filePath);
                return Content(fileContent,"application/json");
            }
            return NotFound();
        }
    }
}
