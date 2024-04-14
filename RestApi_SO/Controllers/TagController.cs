using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestApi_SO.Models;
using RestApiSO;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace RestApi_SO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        [HttpGet]
        [Route("GetTagPercentageAverage")] 
        public string GetTagPercentageAverage()
        {
            DBContext dBContext = new DBContext();
            DataTable dt = dBContext.GetAllTags();
            double CountSum = 0;
            List<TagPercentage> percentage = new List<TagPercentage>();
            foreach (DataRow dr in dt.Rows)
            {
                percentage.Add(new TagPercentage(dr[0].ToString(), Convert.ToDouble(dr[1].ToString())));
                CountSum += Convert.ToDouble(dr[1].ToString());
            }

            percentage.ForEach(x => x.Percentage = (x.Percentage * 100) / CountSum);

            return JsonConvert.SerializeObject(percentage);
        }

        [HttpGet]
        [Route("DownloadTagsAgain")] 
        public void DownloadTagsAgain()
        {
            DBContext.GetTags();
        }
    }
}
