using Firebase.Auth;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestRichard.Enums;

namespace TestRichard.Models
{
   
    public class DownloadRequest
    {
        public List<string> Urls { get; set; }
        // [JsonConverter(typeof(JsonStringEnumConverter))]
        public string DownloadSpeed { get; set; } 
        public int NoOfConcurentDownload { get; set; }
    }
 

    public class DownloadRequestValidator : AbstractValidator<DownloadRequest>
    {
        public DownloadRequestValidator()
        {

            RuleForEach(x => x.Urls)
            .Must(ValidateUrl)
            .WithMessage("'{PropertyValue}' is not a valid URL.");

            RuleFor(x => x.DownloadSpeed)
                .Must(ValidateEnum).WithMessage("DownloadSpeed value must be either Fast or Slow");
        }
        private bool ValidateUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        private bool ValidateEnum(string downloadSpeed)
        {
           return Enum.IsDefined(typeof(DownloadSpeed), downloadSpeed);

        }
    }

}
