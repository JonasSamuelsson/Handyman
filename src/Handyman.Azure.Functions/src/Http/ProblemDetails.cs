using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Dynamic;
using System.Net;

namespace Handyman.Azure.Functions.Http
{
    public class ProblemDetails
    {
        public ProblemDetails(HttpStatusCode status, string title)
        {
            Status = status;
            Title = title;
        }

        public object CustomData { get; set; }
        public string Detail { get; set; }
        public HttpStatusCode Status { get; }
        public string Title { get; }

        private ActionResult ToActionResult()
        {
            dynamic o = new ExpandoObject();

            if (CustomData != null)
                Populate(o, CustomData);

            var status = (int) Status;

            if (Detail != null)
                o.Detail = Detail;

            o.Status = status;
            o.Title = Title;
            o.Type = $"https://httpstatuses.com/{status}";

            return new ObjectResult(o)
            {
                ContentTypes = new MediaTypeCollection {new MediaTypeHeaderValue("application/problem+json")},
                StatusCode = status
            };
        }

        private static void Populate(dynamic o, object customData)
        {
            foreach (var property in customData.GetType().GetProperties())
            {
                o[property.Name] = property.GetValue(customData);
            }
        }

        public static implicit operator ActionResult(ProblemDetails problemDetails)
        {
            return problemDetails.ToActionResult();
        }
    }
}