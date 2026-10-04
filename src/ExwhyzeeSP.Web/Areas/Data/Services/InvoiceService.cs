using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExwhyzeeSP.Web.Areas.Data.Services
{
    public class InvoiceService : IInvoiceService
    {
        public Task Create(Invoice item) => throw new NotImplementedException();
        public Task Delete(int? id) => throw new NotImplementedException();
        public Task Edit(Invoice item) => throw new NotImplementedException();
        public Task<Invoice> Get(int? id) => throw new NotImplementedException();
        public Task<List<Invoice>> GetUserInvoice(string userId) => throw new NotImplementedException();
        public Task<List<Invoice>> GetUserInvoiceWithOutId() => throw new NotImplementedException();
        public Task<List<Invoice>> InvoiceList() => throw new NotImplementedException();
        public Task<List<Invoice>> PaidInvoiceList() => throw new NotImplementedException();
        public Task<List<Invoice>> UnPaidInvoiceList() => throw new NotImplementedException();
    }
}
