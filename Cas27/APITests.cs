using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;

namespace Cas27
{
    class APITests
    {

        private ContactsAPI API;

        [SetUp]
        public void Boot()
        {
            API = new ContactsAPI(
                "https://api.qa.rs/api/",
                TimeSpan.FromSeconds(10)
            );
        }


        [Test]
        public void TestGetContacts()
        {
            ContactsObject Contacts;
            HttpResponseMessage Response;

            API.GetContacts(
                1,
                out Contacts,
                out Response
            );

            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            Assert.AreEqual(25, Contacts.results_per_page);
        }
        
        [Test]
        public void TestGetContact()
        {
            ContactObject Contact;
            HttpResponseMessage Response;

            API.GetContact(1, out Contact, out Response);

            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            Assert.AreEqual(1, Contact.id);
        }

        [Test]
        public void TestPostContact()
        {
            ContactObject Contact;
            HttpResponseMessage Response;

            ContactObject NewContact = new ContactObject();
            NewContact.first_name = "Dejana";
            NewContact.last_name = "Dejanovic";
            NewContact.email = "ddejana@example.org";
            NewContact.phone_number = "012/345-678";

            API.PostContact(NewContact, out Contact, out Response);

            Assert.AreEqual(HttpStatusCode.Created, Response.StatusCode);

            ContactObject VerifyContact;

            API.GetContact(Contact.id, out VerifyContact, out Response);

            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            Assert.AreEqual(VerifyContact.first_name, NewContact.first_name);
            Assert.AreEqual(VerifyContact.last_name, NewContact.last_name);
            Assert.AreEqual(VerifyContact.email, NewContact.email);
            Assert.AreEqual(VerifyContact.phone_number, NewContact.phone_number);
        }


        [TearDown]
        public void Destroy()
        {
            API.Cleanup();
        }
    }
}
