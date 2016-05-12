﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class RuntimeDatabase : WorldDataService
    {
        override public MudObject ReloadObject(String Path)
		{
            Path = Path.Replace('\\', '/');

			if (NamedObjects.ContainsKey(Path))
			{
				var existing = NamedObjects[Path];
				var newObject = CompileObject(Path);
				if (newObject == null)  return null;

                existing.State = ObjectState.Destroyed;
				NamedObjects.Upsert(Path, newObject);
                MudObject.InitializeObject(newObject);

				//Preserve contents
                    foreach (var item in existing.EnumerateObjectsAndRelloc())
                    {
                        newObject.Add(item.Item1, item.Item2);
                        item.Item1.Location = newObject;
                    }
                 
				//Preserve location
				if (existing is MudObject && newObject is MudObject)
				{
					if ((existing as MudObject).Location != null)
					{
                        var loc = existing.Location.RelativeLocationOf(existing);
						MudObject.Move(newObject as MudObject, (existing as MudObject).Location, loc);
						MudObject.Move(existing as MudObject, null, RelativeLocations.None);
					}
				}

                existing.Destroy(false);

				return newObject;
			}
			else
				return GetObject(Path);
		}
    }
}
