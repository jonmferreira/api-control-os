using System;
using System.IO;
using System.Linq;
using FluentValidation;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Api.Validators;

public sealed class UploadPhotoAttachmentRequestValidator : AbstractValidator<UploadPhotoAttachmentRequest>
{
    public UploadPhotoAttachmentRequestValidator()
    {
        RuleFor(request => request.Url)
            .NotEmpty()
            .MaximumLength(500)
            .Must(BeValidUrl)
            .WithMessage("Url must be a valid absolute or relative URL.")
            .Must(HaveAllowedImageExtension)
            .WithMessage("Url must reference an image with extensions .jpg, .jpeg, .png, .gif, .heic or .webp.");

        RuleFor(request => request.Type)
            .NotEmpty()
            .Must(BeValidAttachmentType)
            .WithMessage("Type must be OrderEvidence or ChecklistItemEvidence.");

        When(request => IsOrderEvidence(request.Type), () =>
        {
            RuleFor(request => request.OrderOfServiceId)
                .NotNull()
                .WithMessage("OrderOfServiceId is required when Type is OrderEvidence.");
        });

        When(request => IsChecklistItemEvidence(request.Type), () =>
        {
            RuleFor(request => request.ChecklistResponseItemId)
                .NotNull()
                .WithMessage("ChecklistResponseItemId is required when Type is ChecklistItemEvidence.");
        });
    }

    private static bool BeValidAttachmentType(string type)
    {
        return Enum.TryParse(type, ignoreCase: true, out PhotoAttachmentType _);
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
    }

    private static bool IsOrderEvidence(string type)
    {
        return Enum.TryParse(type, ignoreCase: true, out PhotoAttachmentType parsed)
            && parsed == PhotoAttachmentType.OrderEvidence;
    }

    private static bool IsChecklistItemEvidence(string type)
    {
        return Enum.TryParse(type, ignoreCase: true, out PhotoAttachmentType parsed)
            && parsed == PhotoAttachmentType.ChecklistItemEvidence;
    }

    private static bool HaveAllowedImageExtension(string url)
    {
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
        {
            return false;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".webp" };
        var path = uri.IsAbsoluteUri ? uri.LocalPath : uri.OriginalString;
        var extension = Path.GetExtension(path);

        return allowedExtensions.Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }
}
