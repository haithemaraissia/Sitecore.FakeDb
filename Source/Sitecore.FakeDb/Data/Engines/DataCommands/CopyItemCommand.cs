﻿namespace Sitecore.FakeDb.Data.Engines.DataCommands
{
  using System;
  using System.Linq;
  using System.Threading;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Diagnostics;

  public class CopyItemCommand : Sitecore.Data.Engines.DataCommands.CopyItemCommand, IDataEngineCommand, IDisposable
  {
    private readonly ThreadLocal<DataEngineCommand> innerCommand;

    private readonly ThreadLocal<ItemCreator> itemCreator;

    private bool disposed;

    public CopyItemCommand()
    {
      this.innerCommand = new ThreadLocal<DataEngineCommand> { Value = DataEngineCommand.NotInitialized };
      this.itemCreator = new ThreadLocal<ItemCreator>();
    }

    ~CopyItemCommand()
    {
      this.Dispose(false);
    }

    public virtual void Initialize(DataEngineCommand command)
    {
      this.innerCommand.Value = command;
    }

    public ItemCreator ItemCreator
    {
      get { return this.itemCreator.Value ?? (this.itemCreator.Value = new ItemCreator(this.innerCommand.Value.DataStorage)); }
      set { this.itemCreator.Value = value; }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (!disposing)
      {
        return;
      }

      if (null != this.innerCommand)
      {
        this.innerCommand.Dispose();
      }

      if (null != this.itemCreator)
      {
        this.itemCreator.Dispose();
      }

      this.disposed = true;
    }

    protected override Sitecore.Data.Engines.DataCommands.CopyItemCommand CreateInstance()
    {
      return this.innerCommand.Value.CreateInstance<Sitecore.Data.Engines.DataCommands.CopyItemCommand, CopyItemCommand>();
    }

    protected override Item DoExecute()
    {
      this.ItemCreator.Create(this.CopyName, this.CopyId, this.Source.TemplateID, this.Database, this.Destination);

      var dataStorage = this.innerCommand.Value.DataStorage;

      var fakeItem = dataStorage.GetFakeItem(this.Source.ID);
      var fakeCopy = dataStorage.GetFakeItem(this.CopyId);

      this.CopyFields(fakeItem, fakeCopy);

      var copy = dataStorage.GetSitecoreItem(this.CopyId, this.Source.Language);

      if (!this.Deep)
      {
        return copy;
      }

      foreach (Item child in this.Source.Children)
      {
        ItemManager.CopyItem(child, copy, this.Deep, child.Name, ID.NewID);
      }

      return copy;
    }

    protected virtual void CopyFields(DbItem source, DbItem copy)
    {
      Assert.ArgumentNotNull(source, "source");
      Assert.ArgumentNotNull(copy, "copy");

      foreach (var field in source.Fields)
      {
        this.CopyField(field, copy);
      }
    }

    protected virtual void CopyField(DbField field, DbItem copy)
    {
      copy.Fields.Add(new DbField(field.Name, field.ID)
      {
        Shared = field.Shared,
        Type = field.Type
      });

      if (field.Shared)
      {
        copy.Fields[field.ID].Value = field.Value;
      }
      else
      {
        foreach (var fieldValue in field.Values)
        {
          var language = fieldValue.Key;
          var versions = fieldValue.Value.ToDictionary(v => v.Key, v => v.Value);

          copy.Fields[field.ID].Values.Add(language, versions);
        }
      }
    }
  }
}