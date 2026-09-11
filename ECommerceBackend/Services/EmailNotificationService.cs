using System.Net;
using System.Net.Mail;
using ECommerceBackend.Email;
using ECommerceBackend.Models;
using ECommerceBackend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IOptions<EmailSettings> options,
        ILogger<EmailNotificationService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(Order order)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation(
                "Order email notification is disabled. OrderId: {OrderId}",
                order.Id);
            return;
        }

        ValidateSettings();

        using var message = new MailMessage
        {
            From = new MailAddress(
                _settings.FromAddress,
                _settings.FromName),
            Subject = $"Order #{order.Id} confirmed",
            IsBodyHtml = true,
            Body = BuildOrderBody(order)
        };

        message.To.Add(new MailAddress(order.User.Email, order.User.Name));

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            client.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password);
        }

        await client.SendMailAsync(message);

        _logger.LogInformation(
            "Order confirmation email sent. OrderId: {OrderId}",
            order.Id);
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.FromAddress))
        {
            throw new InvalidOperationException(
                "EmailSettings Host and FromAddress are required when email is enabled.");
        }
    }

    private static string BuildOrderBody(Order order)
    {
        var rows = string.Join(
            string.Empty,
            order.OrderItems.Select(item =>
                $"""
                <tr>
                    <td style="padding:8px;border-bottom:1px solid #eee;">
                        {WebUtility.HtmlEncode(item.Product?.Name ?? "Product")}
                    </td>
                    <td style="padding:8px;border-bottom:1px solid #eee;">
                        {item.Quantity}
                    </td>
                    <td style="padding:8px;border-bottom:1px solid #eee;">
                        ₹{item.Price:N2}
                    </td>
                </tr>
                """));

        return $"""
            <h2>Thank you for your order, {WebUtility.HtmlEncode(order.User.Name)}.</h2>
            <p>Your payment was successful and order <strong>#{order.Id}</strong> has been confirmed.</p>
            <table style="border-collapse:collapse;width:100%;max-width:600px;">
                <thead>
                    <tr>
                        <th style="text-align:left;padding:8px;">Product</th>
                        <th style="text-align:left;padding:8px;">Quantity</th>
                        <th style="text-align:left;padding:8px;">Price</th>
                    </tr>
                </thead>
                <tbody>{rows}</tbody>
            </table>
            <p><strong>Total: ₹{order.TotalAmount:N2}</strong></p>
            <p>Delivery address: {WebUtility.HtmlEncode(order.ShippingAddress)}</p>
            """;
    }
}
