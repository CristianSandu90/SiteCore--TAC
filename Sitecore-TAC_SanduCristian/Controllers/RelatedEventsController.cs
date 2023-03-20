using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using events.tac.local.Business;

namespace events.tac.local.Controllers
{
    public class RelatedEventsController : Controller
    {
        private readonly RelatedEventsProvider _provider;

        private RelatedEventsController() : this(new RelatedEventsProvider())
        {
        }

        public RelatedEventsController(RelatedEventsProvider provider)
        {
            _provider = provider;
        }

        // GET: RelatedEvents
        public ActionResult Index()
        {
            return View(_provider.GetRelatedEvents());
        }
    }
}