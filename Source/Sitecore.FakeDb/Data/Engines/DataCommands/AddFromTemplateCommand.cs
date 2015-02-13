namespace Sitecore.FakeDb.Data.Engines.DataCommands
{
  using System;
  using System.Threading;
  using Sitecore.Data.Items;

  public class AddFromTemplateCommand : Sitecore.Data.Engines.DataCommands.AddFromTemplateCommand, IDataEngineCommand, IDisposable
  {
    private readonly ThreadLocal<DataEngineCommand> innerCommand;

    private readonly ThreadLocal<ItemCreator> itemCreator;

    private bool disposed;

    public AddFromTemplateCommand()
    {
      this.innerCommand = new ThreadLocal<DataEngineCommand> { Value = DataEngineCommand.NotInitialized };
      this.itemCreator = new ThreadLocal<ItemCreator>();
    }

    ~AddFromTemplateCommand()
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

      if (null != this.itemCreator)
      {
        this.itemCreator.Dispose();
      }

      if (null != this.innerCommand)
      {
        this.innerCommand.Dispose();
      }

      this.disposed = true;
    }

    protected override Sitecore.Data.Engines.DataCommands.AddFromTemplateCommand CreateInstance()
    {
      return this.innerCommand.Value.CreateInstance<Sitecore.Data.Engines.DataCommands.AddFromTemplateCommand, AddFromTemplateCommand>();
    }

    protected override Item DoExecute()
    {
      return this.ItemCreator.Create(this.ItemName, this.NewId, this.TemplateId, this.Database, this.Destination, true);
    }
  }
}