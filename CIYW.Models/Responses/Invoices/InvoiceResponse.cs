﻿using CIYW.Const.Enums;
using CIYW.Models.Responses.Base;
using CIYW.Models.Responses.Categories;
using CIYW.Models.Responses.Currencies;
using CIYW.Models.Responses.Notes;

namespace CIYW.Models.Responses.Invoices;

public class InvoiceResponse: BaseWithDateEntityResponse
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryResponse Category { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse Currency { get; set; }
    
    public DateTime Date { get; set; }
    public Guid NoteId { get; set; }
    
    public NoteResponse Note { get; set; }
    
    public InvoiceTypeEnum Type { get; set; }
}