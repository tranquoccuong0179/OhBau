using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace OhBau.API.Controllers
{
    [ApiController]
    [Route("api/v1/statictis")]
    public class StatictisController : Controller
    {
        [HttpGet("get-visit-count")]
        public async Task<IActionResult> GetVisited()
        {
            Dictionary<string,int> countVisited = new Dictionary<string,int>();

            const string _filePath = "visitcount.json";
            var json = await System.IO.File.ReadAllTextAsync(_filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            foreach(var item in data)
            {
                countVisited[item.Key] = item.Value.Count;
            }
            return Ok(countVisited);
        }
    }
}
