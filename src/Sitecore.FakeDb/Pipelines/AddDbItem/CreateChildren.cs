﻿namespace Sitecore.FakeDb.Pipelines.AddDbItem
{
  public class CreateChildren
  {
    public virtual void Process(AddDbItemArgs args)
    {
      var item = args.DbItem;
      var dataStorage = args.DataStorage;

      foreach (var child in item.Children)
      {
        child.ParentID = item.ID;
        child.FullPath = item.FullPath + "/" + child.Name;
        dataStorage.AddFakeItem(child);
      }
    }
  }
}