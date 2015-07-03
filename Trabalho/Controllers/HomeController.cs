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


        //Controle que gerencia a pagina inicial
        public ActionResult Index()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBContext>());


            var subscribers = _webService.GetSubscribers().ToList();

            var model = new List<Subscriber>();

            //Recebe lista de usuarios
            subscribers.ForEach(s => {
                var sTemp = new Subscriber();
                sTemp.Email = s.Email;
                sTemp.IsActive = (bool) s.IsActive;
                sTemp.Name = s.Name;
                model.Add(sTemp);
            });

            return View(model.ToArray());
        }


        //Controle que permite a edicao de dados de um usuario, esta funcao salva os dados
        [HttpPost]
        public ActionResult Edit(Subscriber subscriber)
        {
            //Edicao feita diretamente atraves do webservice
            _webService.AddorUpdateSubscriber(subscriber.Name, subscriber.Email, subscriber.IsActive);

            return RedirectToAction("Index", this);
        }

        //Controle que permite a edicao de dados de um usuario, esta funcao exibe os dados na tela
        public ActionResult Edit(string email)
        {
            //Se email nao existe lanca erro na tela
            if(null == email)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var s = _webService.GetSubscriberByEmail(email);

            var subscriber = new Subscriber();

            subscriber.Email = s.Email;
            subscriber.Name = s.Name;
            subscriber.IsActive = s.IsActive;

            return View(subscriber);
        }

        //Funcao que permite a ativacao e inativacao de uma determinada conta de usuario
        public ActionResult ToggleSubscription(string email)
        {
            //chamada ao webservice
            var s = _webService.TooggleSubscriberActive(email);

            return RedirectToAction("Login", "Home", new { email = email});
        }


        //Controle que permite a criacao de dados de um usuario, esta funcao exibe os dados na tela
        public ActionResult Create()
        {
           return View();
        }

        //Controle que permite a criacao de dados de um usuario, esta funcao salva os dados no webservice
        [HttpPost]
        public ActionResult Create(Subscriber subscriber)
        {
            //chamada ao webservice
            _webService.AddorUpdateSubscriber(subscriber.Name, subscriber.Email, subscriber.IsActive);

            return RedirectToAction("Index", this);
        }

        //Funcao que cria um formulario para o login de um usuario e 
        //Gerencia se o usuario existe ou não. Se não existe retorna um erro
        public ActionResult Login(string email)
        {

            if(!ModelState.IsValid)
                return RedirectToAction("Index", this);

            var publisher = new Publisher();
            //chamada ao webservice
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


        //Funcao que recebe uma mensagem de um determinado usuario e envia para o webservice
        //O webservice vai tratar de enviar os email para os outros usuarios
        [HttpPost]
        public ActionResult SendMessage(Publisher publisher)
        {


            _webService.AddMessage(publisher.Message, publisher.SubscriberEmail);



            return RedirectToAction("Index");
        }
    }
}
