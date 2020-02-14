using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace CustomerDB
{
    public class CustomerManager
    {
        public IEnumerable<Customer> GetAllCustmers()
        {
            List<Customer> customers = null;
            using (APIDemoEntities entities = new APIDemoEntities())
            {
                customers = entities.CUSTOMERs.AsEnumerable().Select(x => new Customer
                {
                    ID = x.ID,
                    Name = x.FirstName + " " + x.LastName,
                    ContactNumber = Convert.ToInt64(x.ContactNumber),
                    Address = x.Address
                }).ToList();
            }

            return customers;
        }

        public Customer GetCustmerByID(int id)
        {
            Customer customer = null;
            using (APIDemoEntities entities = new APIDemoEntities())
            {
                customer = entities.CUSTOMERs.AsEnumerable().Where(x => x.ID == id).Select(x => new Customer
                {
                    ID = x.ID,
                    Name = x.FirstName + " " + x.LastName,
                    ContactNumber = Convert.ToInt64(x.ContactNumber),
                    Address = x.Address
                }).FirstOrDefault();
            }

            return customer;
        }
    }
}
