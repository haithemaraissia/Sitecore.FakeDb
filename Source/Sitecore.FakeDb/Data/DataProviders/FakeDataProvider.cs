﻿namespace Sitecore.FakeDb.Data.DataProviders
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.DataProviders;
  using Sitecore.Data.Templates;
  using Sitecore.FakeDb.Data.Engines;
  using Sitecore.Globalization;
  using CallContext = Sitecore.Data.DataProviders.CallContext;

  public class FakeDataProvider : DataProvider, IRequireDataStorage
  {
    private DataStorage dataStorage;

    public FakeDataProvider()
    {
    }

    internal FakeDataProvider(DataStorage dataStorage)
    {
      this.dataStorage = dataStorage;
    }

    public virtual DataStorage DataStorage
    {
      get { return this.dataStorage; }
    }

    public virtual void SetDataStorage(DataStorage dataStorage)
    {
      this.dataStorage = dataStorage;
    }

    public override IdCollection GetTemplateItemIds(CallContext context)
    {
      var ids = this.DataStorage.FakeTemplates.Select(t => t.Key).ToArray();

      return new IdCollection { ids };
    }

    public override TemplateCollection GetTemplates(CallContext context)
    {
      var templates = new TemplateCollection();

      foreach (var ft in this.DataStorage.FakeTemplates.Values)
      {
        var builder = new Template.Builder(ft.Name, ft.ID, templates);
        var section = builder.AddSection("Data", ID.NewID);
        section.AddField("__Base template", FieldIDs.BaseTemplate).SetType("tree list");

        foreach (var field in ft.Fields)
        {
          section.AddField(field.Name, field.ID).SetType(field.Type);
        }

        builder.SetBaseIDs(string.Join("|", ft.BaseIDs ?? new ID[] { } as IEnumerable<ID>));

        templates.Add(builder.Template);
      }

      return templates;
    }

    public override LanguageCollection GetLanguages(CallContext context)
    {
      return new LanguageCollection { Language.Parse("en-US") };
    }
  }
}