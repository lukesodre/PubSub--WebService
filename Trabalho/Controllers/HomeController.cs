using Core.DAL;
using Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace Trabalho.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private DBContext db = new DBContext();
        PubSubService.Service _webService = new PubSubService.Service();



        public ActionResult Index()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBContext>());


            var subscribers = _webService.GetSubscribers().ToList();

            var model = new List<Subscriber>();

            subscribers.ForEach(s => {
                var sTemp = new Subscriber();
                sTemp.Email = s.Email;
                sTemp.IsActive = (bool) s.IsActive;
                sTemp.Name = s.Name;
                model.Add(sTemp);
            });

            return View(model.ToArray());
        }



        [HttpPost]
        public ActionResult Edit(Subscriber subscriber)
        {

            _webService.AddorUpdateSubscriber(subscriber.Name, subscriber.Email, subscriber.IsActive);

            return RedirectToAction("Index", this);
        }


        public ActionResult Edit(string email)
        {

            if(null == email)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var s = _webService.GetSubscriberByEmail(email);

            var subscriber = new Subscriber();

            subscriber.Email = s.Email;
            subscriber.Name = s.Name;
            subscriber.IsActive = s.IsActive;

            return View(subscriber);
        }

        
        public ActionResult ToggleSubscription(string email)
        {

            var s = _webService.TooggleSubscriberActive(email);

            return RedirectToAction("Login", "Home", new { email = email});
        }

        public ActionResult Create()
        {
           return View();
        }

        [HttpPost]
        public ActionResult Create(Subscriber subscriber)
        {
            _webService.AddorUpdateSubscriber(subscriber.Name, subscriber.Email, subscriber.IsActive);

            return RedirectToAction("Index", this);
        }


        public ActionResult Login(string email)
        {

            if(!ModelState.IsValid)
                return RedirectToAction("Index", this);

            var publisher = new Publisher();
            var s = _webService.GetSubscriberByEmail(email);

            if(null == s){
                ModelState.AddModelError("", "Email não encontrado");
                var subscribers = _webService.GetSubscribers().ToList();

                var model = new List<Subscriber>();

                subscribers.ForEach(sub =>
                {
                    var sTemp = new Subscriber();
                    sTemp.Email = sub.Email;
                    sTemp.IsActive = (bool)sub.IsActive;
                    sTemp.Name = sub.Name;
                    model.Add(sTemp);
                });

                return View("Index",model.ToArray());
            }
            var subscriber = new Subscriber();

            subscriber.Email = s.Email;
            subscriber.Name = s.Name;
            subscriber.IsActive = s.IsActive;
            
            
            publisher.SubscriberEmail = subscriber.Email;

            ViewBag.Subscriber = subscriber;

            return View("SendMessage",publisher);
        }

        [HttpPost]
        public ActionResult SendMessage(Publisher publisher)
        {


            _webService.AddMessage(publisher.Message, publisher.SubscriberEmail);



            return RedirectToAction("Index");
        }
    }
}
