using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using ClientContactManager.Application.Commands.Contacts;
using ClientContactManager.Application.Queries.Contacts;

namespace ClientContactManager.WebUI.Controllers;

public class ContactController : Controller
{
    private readonly IMediator _mediator;
    public ContactController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var contacts = await _mediator.Send(new GetAllContactsQuery());
        return View(contacts);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string surname, string email)
    {
        try
        {
            var id = await _mediator.Send(new CreateContactCommand(name, surname, email));
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ValidationException ex)
        {
            foreach (var err in ex.Errors)
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);

            ViewBag.Name = name;
            ViewBag.Surname = surname;
            ViewBag.Email = email;
            return View();
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var contact = await _mediator.Send(new GetContactByIdQuery(id));
        if (contact is null) return NotFound();

        ViewBag.AvailableClients = await _mediator.Send(new GetAvailableClientsForContactQuery(id));
        return View(contact);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkClient(int contactId, int clientId)
    {
        await _mediator.Send(new LinkClientToContactCommand(contactId, clientId));
        return RedirectToAction(nameof(Edit), new { id = contactId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
    {
        await _mediator.Send(new UnlinkClientFromContactCommand(contactId, clientId));
        return RedirectToAction(nameof(Edit), new { id = contactId });
    }
}
