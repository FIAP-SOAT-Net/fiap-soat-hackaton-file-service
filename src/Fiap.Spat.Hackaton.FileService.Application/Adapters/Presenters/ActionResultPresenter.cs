using Fiap.Spat.Hackaton.FileService.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Spat.Hackaton.FileService.Application.Adapters.Presenters;

public static class ActionResultPresenter
{
    public static ActionResult ToActionResult(Response result) => new ObjectResult(result) { StatusCode = (int?) result.StatusCode };

    public static ActionResult ToActionResult<T>(Response<T> result) => new ObjectResult(result) { StatusCode = (int?) result.StatusCode };
}
