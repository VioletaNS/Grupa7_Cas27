using System.Collections.Generic;

namespace Cas27
{
    class ContactsObject
    {
        public int page { get; set; }
        public int results_per_page { get; set; }
        public List<ContactObject> results { get; set; }
    }
}
