using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WineMan.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string WineBrandSelect()
        {
            List <Wine_Brand> brands = Wine_Brand.GetAllRecords();
            string selectString = "<select>";
            foreach (Wine_Brand brand in brands)
            {
                selectString += "<option value='" + brand.id + "'>" + brand.name + "</option>";
            }

            selectString += "</select>";
            return selectString;
        }

        [HttpGet]
        public string WineCategorySelect()
        {
            List<Wine_Category> categories = Wine_Category.GetAllRecords(true);
            string selectString = "<select>";
            foreach (Wine_Category category in categories)
            {
                selectString += "<option value='" + category.id + "'>" + category.name + "</option>";
            }

            selectString += "</select>";
            return selectString;
        }

        [HttpGet]
        public string StepSelect()
        {
            List<Step> allsteps = Step.GetAllRecords();
            string selectString = "<select>";
            foreach (Step step in allsteps)
            {
                selectString += "<option value='" + step.id + "'>" + step.name + "</option>";
            }

            selectString += "</select>";
            return selectString;
        }
	}
}