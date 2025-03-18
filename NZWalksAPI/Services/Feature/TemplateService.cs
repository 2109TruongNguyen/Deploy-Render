namespace NZWalksAPI.Services.Feature;

public class TemplateService
{
    public async Task<string> GetThankYouTemplateAsync(string templatePath, string userName)
    {
        var templateContent = await File.ReadAllTextAsync(templatePath);
        return templateContent.Replace("{{UserName}}", userName);
    }
}