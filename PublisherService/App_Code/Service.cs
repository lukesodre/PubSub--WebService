using Core.DAL;
using Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Net.Mail;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    MailAddress fromAddress = new MailAddress("usuarioteste.lucas@gmail.com", "Teste");
    // MailAddress toAddress = new MailAddress("to@example.com", "To Name");
    string fromPassword = "testebonitao";
    string subject = "New Message published";
    string body = "Body";
    List<Subscriber> subscribers;
    private DBContext db = new DBContext();


    public Service()
    {
        Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DBContext>());
        subscribers = new List<Subscriber>();

 

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool AddMessage(string message, string email)
    {
        var subscriber = db.Subscribers.FirstOrDefault(s => string.Compare(s.Email, email) == 0);

        if (null == subscriber)
            return false;

        var p = new Publisher();

        p.Message = message;
        p.Subscriber = subscriber;
        db.Publishers.Add(p);
        db.SaveChanges();

        var emailsList = new List<MailAddress>();

        subscribers = db.Set<Subscriber>().ToList();
        var emails = new List<string>();
        subscribers.ForEach(s =>
        {
            if (s.IsActive && (string.Compare(s.Email,email) != 0))
                emails.Add(s.Email);
        });

        emails.ToList().ForEach(e => emailsList.Add(new MailAddress(e, subscriber.Name)));


        #region Send Email
        foreach (var toAddress in emailsList)
        {

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var messageEmail = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = message
            })
            {
                try
                {
                    smtp.Send(messageEmail);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error Web Service " + ex.Message);
                    return false;
                }
            }
        }
        #endregion

        return true;
    }

    [WebMethod]
    public bool AddorUpdateSubscriber(string name, string email, bool isActive)
    {
        var subscriber = db.Subscribers.FirstOrDefault(s => string.Compare(s.Email, email) == 0);

        if (null == subscriber)
        {
            subscriber = new Subscriber();
            subscriber.Email = email;
            subscriber.IsActive = isActive;
            subscriber.Name = name;
            db.Subscribers.Add(subscriber);
            db.SaveChanges();
            return true;
        }

        subscriber.Email = email;
        subscriber.IsActive = isActive;
        subscriber.Name = name;


        db.SaveChanges();

        return true;
    }


    [WebMethod]
    public List<Publisher> GetMessagesByEmail(string email)
    {
        var publisherList = new List<Publisher>();

        publisherList = db.Set<Publisher>().ToList().FindAll(p => string.Compare(p.SubscriberEmail, email) == 0);

        if (publisherList.Count == 0)
            return null;

        return publisherList;
    }


    [WebMethod]
    public bool TooggleSubscriberActive(string email)
    {
        var subscriber = new Subscriber();
        subscriber = db.Subscribers.FirstOrDefault(s => string.Compare(s.Email, email) == 0);

        if (null == subscriber)
            return false;
        subscriber.IsActive = !subscriber.IsActive;


        db.SaveChanges();

        return true;
    }


    [WebMethod]
    public Subscriber GetSubscriberByEmail(string email)
    {
        var subscriber = db.Subscribers.FirstOrDefault(s => string.Compare(s.Email, email) == 0);

        return subscriber;

    }


    [WebMethod]
    public List<Publisher> GetAllMessages()
    {

        return db.Set<Publisher>().ToList();
    }

    [WebMethod]
    public List<Subscriber> GetSubscribers()
    {
        subscribers = db.Set<Subscriber>().ToList();
        return subscribers.ToList();
    }

    [WebMethod]
    public List<string> GetEmailList()
    {
        subscribers = db.Set<Subscriber>().ToList();
        var emails = new List<string>();
        subscribers.ForEach(s => emails.Add(s.Email));
        return emails.ToList();
    }

    [WebMethod]
    public List<Subscriber> GetActiveSubscribers()
    {

        return db.Subscribers.ToList().FindAll(s => s.IsActive == true);
    }

}