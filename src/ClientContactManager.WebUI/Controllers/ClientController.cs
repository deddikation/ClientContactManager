using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using ClientContactManager.Application.Commands.Clients;
using ClientContactManager.Application.Queries.Clients;

namespace ClientContactManager.WebUI.Controllers;

public class ClientController : Controller
{
    private readonly IMediator _mediator;
    public ClientController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index()
    {
        var clients = await _mediator.Send(new GetAllClientsQuery());
        return View(clients);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name)
    {
        try
        {
            var id = await _mediator.Send(new CreateClientCommand(name));
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (ValidationException ex)
        {
            foreach (var err in ex.Errors)
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            return View();
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = await _mediator.Send(new GetClientByIdQuery(id));
        if (client is null) return NotFound();

        ViewBag.AvailableContacts = await _mediator.Send(new GetAvailableContactsForClientQuery(id));
        return View(client);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkContact(int clientId, int contactId)
    {
        await _mediator.Send(new LinkContactToClientCommand(clientId, contactId));
        return RedirectToAction(nameof(Edit), new { id = clientId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
    {
        await _mediator.Send(new UnlinkContactFromClientCommand(clientId, contactId));
        return RedirectToAction(nameof(Edit), new { id = clientId });
    }
}
