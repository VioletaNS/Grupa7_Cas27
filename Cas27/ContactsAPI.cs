using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cas27
{
    class ContactsAPI
    {
        private HttpClient Client;

        public ContactsAPI(String BaseURL, TimeSpan Timeout)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(BaseURL);
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
            Client.Timeout = Timeout;
        }

        public void GetContacts(int Page, out ContactsObject Contacts, out HttpResponseMessage Response)
        {
            Response = Client.GetAsync("contacts/" + Page.ToString()).Result;
            if (Response.IsSuccessStatusCode)
            {
                Contacts = Response.Content.ReadAsAsync<ContactsObject>().Result;
            } else
            {
                Contacts = new ContactsObject();
            }
        }

        public void GetContact(int Id, out ContactObject Contact, out HttpResponseMessage Response)
        {
            Response = Client.GetAsync("contact/" + Id.ToString()).Result;
            if (Response.IsSuccessStatusCode)
            {
                Contact = Response.Content.ReadAsAsync<ContactObject>().Result;
            } else
            {
                Contact = new ContactObject();
            }
        }

        public void PostContact(
            ContactObject NewContact,
            out ContactObject InsertedContact,
            out HttpResponseMessage Response
        ) {
            Response = Client.PostAsJsonAsync<ContactObject>("contact", NewContact).Result;
            if (Response.IsSuccessStatusCode)
            {
                InsertedContact = Response.Content.ReadAsAsync<ContactObject>().Result;
            }
            else
            {
                InsertedContact = new ContactObject();
            }
        }

        public void PutContact(int Id, ContactObject UpdatedContact, out HttpResponseMessage Response)
        {
            Response = Client.PutAsJsonAsync<ContactObject>("contact/" + Id.ToString(), UpdatedContact).Result;
        }

        public void DeleteContact(int Id, out HttpResponseMessage Response)
        {
            Response = Client.DeleteAsync("contact/" + Id.ToString()).Result;
        }

        public void Cleanup()
        {
            Client.Dispose();
        }
    }
}
