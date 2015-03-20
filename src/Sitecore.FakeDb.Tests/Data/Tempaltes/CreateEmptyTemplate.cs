namespace Sitecore.FakeDb.Tests.Data.Tempaltes
{
  using System;
  using System.Linq;
  using FluentAssertions;
  using Sitecore.Collections;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.FakeDb.Sites;
  using Sitecore.Sites;
  using Xunit;

  [Trait("Templates", "Create empty template")]
  public class CreateEmptyTemplate : IDisposable
  {
    private readonly Db db;

    private readonly TemplateItem templateItem;

    public CreateEmptyTemplate()
    {
      this.db = new Db();

      var templates = this.db.GetItem(ItemIDs.TemplateRoot);

      // TODO:[Hack] Needs ContentDatabase to create a template...
      using (new SiteContextSwitcher(new FakeSiteContext(new StringDictionary { { "content", "master" } })))
      {
        this.templateItem = this.db.Database.Templates.CreateTemplate("My Template", templates);
      }
    }

    [Fact(DisplayName = "Is Template")]
    public void IsTemplate()
    {
      TemplateManager.IsTemplate(this.templateItem).Should().BeTrue();
    }

    [Fact(DisplayName = "Can get from TemplateManager by template item")]
    public void GetTemplate()
    {
      TemplateManager.GetTemplate(this.templateItem).Should().NotBeNull();
    }

    [Fact(DisplayName = "Can get from TemplateManager by id", Skip = "To be implemented.")]
    public void GetTemplateById()
    {
      TemplateManager.GetTemplate(this.templateItem.ID, this.db.Database).Should().NotBeNull();
    }

    [Fact(DisplayName = "Created in Templates root")]
    public void ParentIsSitecoreTemplates()
    {
      this.templateItem.InnerItem.Paths.FullPath.Should().Be("/sitecore/templates/My Template");
    }

    [Fact(DisplayName = "Template item name is set")]
    public void TemplateItemNameSet()
    {
      this.db.GetItem(this.templateItem.ID).Name.Should().Be("My Template");
    }

    [Fact(DisplayName = "Template name is set", Skip = "To be implemented.")]
    public void TemplateNameSet()
    {
      TemplateManager.GetTemplate(this.templateItem).Name.Should().Be("My Template");
    }

    [Fact(DisplayName = "Based on Standard Template")]
    public void BaseTemplateIsStandardTemplate()
    {
      TemplateManager.GetTemplate(this.templateItem).BaseIDs.Single().Should().Be(TemplateIDs.StandardTemplate);
    }

    public void Dispose()
    {
      this.db.Dispose();
    }
  }
}