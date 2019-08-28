using System.Dynamic;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Handyman.Azure.Functions
{
   public class ProblemDetails
   {
      public ProblemDetails(HttpStatusCode status, string title)
      {
         Status = status;
         Title = title;
      }

      public string Detail { get; set; }
      public HttpStatusCode Status { get; }
      public string Title { get; }

      private ActionResult ToActionResult()
      {
         dynamic o = new ExpandoObject();

         var status = (int)Status;

         if (Detail != null)
            o.Detail = Detail;

         o.Status = status;
         o.Title = Title;
         o.Type = $"https://httpstatuses.com/{status}";

         return new ObjectResult(o) { StatusCode = status };
      }

      public static implicit operator ActionResult(ProblemDetails problemDetails)
      {
         return problemDetails.ToActionResult();
      }
   }
}