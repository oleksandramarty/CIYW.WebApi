﻿using CIYW.Domain;
using CIYW.Domain.Models.Invoice;
using CIYW.Domain.Models.Note;
using CIYW.Domain.Models.User;
using CIYW.Interfaces;
using CIYW.Kernel.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Repositories;

public class TransactionRepository: ITransactionRepository
{
    private readonly DataContext context;

    public TransactionRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task AddInvoiceAsync(
        Guid userId,
        Invoice invoice,
        Note note,
        CancellationToken cancellationToken)
    {
        using (var transaction = await this.context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                UserBalance userBalance =
                    await this.context.UserBalances.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

                userBalance.AddInvoice(invoice);
                
                if (note != null)
                {
                    await this.context.Notes.AddAsync(note, cancellationToken);                    
                }
                
                await this.context.Invoices.AddAsync(invoice, cancellationToken);

                this.context.UserBalances.Update(userBalance);

                await this.context.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }
    }
    
    public async Task UpdateInvoiceAsync(
        Guid userId,
        Invoice invoice,
        Invoice updatedInvoice,
        CancellationToken cancellationToken)
    {
        EntityExtension.HasAccess(invoice, userId);
        
        using (var transaction = await this.context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                UserBalance userBalance =
                    await this.context.UserBalances.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

                userBalance.UpdateInvoice(invoice, updatedInvoice);
                
                this.context.Invoices.Update(updatedInvoice);

                this.context.UserBalances.Update(userBalance);

                await this.context.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }
    }
    
    public async Task DeleteInvoiceAsync(
        Guid userId,
        Invoice invoice,
        CancellationToken cancellationToken)
    {
        EntityExtension.HasAccess(invoice, userId);
        
        using (var transaction = await this.context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                UserBalance userBalance =
                    await this.context.UserBalances.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

                userBalance.DeleteInvoice(invoice);

                if (invoice.NoteId.HasValue)
                {
                    Note note = await this.context.Notes.FirstOrDefaultAsync(n => n.Id == invoice.NoteId, cancellationToken);
                    this.context.Notes.Remove(note);
                }
                
                this.context.Invoices.Remove(invoice);

                this.context.UserBalances.Update(userBalance);

                await this.context.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }
    }
}