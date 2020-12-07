using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        [Test]
        [TestCase(null)]
        public void TestUpdateContact(int? ContactId = null)
        {
            ContactObject Contact;
            ContactsObject Contacts;
            HttpResponseMessage Response;

            if (!ContactId.HasValue) {
                API.GetContacts(1, out Contacts, out Response);
                Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
                ContactId = Contacts.results.First().id;
            }

            API.GetContact(ContactId.Value, out Contact, out Response);
            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);

            Contact.phone_number = "000-0000-000";

            API.PutContact(ContactId.Value, Contact, out Response);
            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
        }

        [Test]
        [TestCase(null)]
        public void TestDeleteContact(int? ContactId = null)
        {
            ContactObject Contact;
            ContactsObject Contacts;
            HttpResponseMessage Response;

            if (!ContactId.HasValue)
            {
                API.GetContacts(1, out Contacts, out Response);
                Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
                ContactId = Contacts.results.First().id;
            }

            TestContext.WriteLine("Trying to delete user with id: {0}", ContactId);
            API.DeleteContact(ContactId.Value, out Response);
            Assert.AreEqual(HttpStatusCode.NoContent, Response.StatusCode);
            
            API.GetContact(ContactId.Value, out Contact, out Response);
            Assert.AreEqual(HttpStatusCode.NotFound, Response.StatusCode);
            TestContext.WriteLine("Successfully deleted user with id: {0}", ContactId);
        }

        [Test]
        public void TestCreateUpdateDeleteUser()
        {
            ContactObject Contact;
            ContactObject VerifyContact;
            ContactObject UpdateContact = new ContactObject();
            ContactObject NewContact = new ContactObject();
            HttpResponseMessage Response;

            NewContact.first_name = "Bob";
            NewContact.last_name = "Buttons";
            NewContact.email = "bob.buttons@example.net";
            NewContact.phone_number = "1-800-555-12345";

            API.PostContact(NewContact, out Contact, out Response);
            Assert.AreEqual(HttpStatusCode.Created, Response.StatusCode);
            TestContext.WriteLine("Created user with id: {0}", Contact.id);

            API.GetContact(Contact.id, out VerifyContact, out Response);
            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            Assert.AreEqual(VerifyContact.first_name, NewContact.first_name);
            Assert.AreEqual(VerifyContact.last_name, NewContact.last_name);
            Assert.AreEqual(VerifyContact.email, NewContact.email);
            Assert.AreEqual(VerifyContact.phone_number, NewContact.phone_number);

            UpdateContact = VerifyContact;
            UpdateContact.phone_number = "1-800-555-6666";

            API.PutContact(UpdateContact.id, UpdateContact, out Response);
            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            TestContext.WriteLine("Updated user {0} information", UpdateContact.id);

            API.GetContact(UpdateContact.id, out VerifyContact, out Response);
            Assert.AreEqual(HttpStatusCode.OK, Response.StatusCode);
            Assert.AreEqual(VerifyContact.first_name, UpdateContact.first_name);
            Assert.AreEqual(VerifyContact.last_name, UpdateContact.last_name);
            Assert.AreEqual(VerifyContact.email, UpdateContact.email);
            Assert.AreEqual(VerifyContact.phone_number, UpdateContact.phone_number);
            TestContext.WriteLine("Verified user {0} information update", UpdateContact.id);

            Thread.Sleep(1000);

            API.DeleteContact(UpdateContact.id, out Response);
            Assert.AreEqual(HttpStatusCode.NoContent, Response.StatusCode);

            API.GetContact(UpdateContact.id, out Contact, out Response);
            TestContext.Write("Deleting user with id: {0} ... ", UpdateContact.id);
            Assert.AreEqual(HttpStatusCode.NotFound, Response.StatusCode);
            TestContext.Write("Deleted");
        }


        [TearDown]
        public void Destroy()
        {
            API.Cleanup();
        }
    }
}
