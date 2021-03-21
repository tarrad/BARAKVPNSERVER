using BARAKVPNSERVER.Helpers;
using BARAKVPNSERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public interface IInvoiceRepository
    {
        public Task<int> AddClient(InvoiceDetail invoice);
        public Task<InternalInvoiceResponse> CreateInvoice(CreateInvoiceDocumentReq invoice, User user);
        public Task<List<InvoicesDocumentDTO>> GetInvoiceByGroupId(int groupId, int page);
    }
}
