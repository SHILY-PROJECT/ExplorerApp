﻿using System;
using System.IO;
using System.Collections.Generic;
using ExplorerApp.Models;
using System.Linq;

namespace ExplorerApp
{
    internal class DataStore
    {
        private static DataStore _instance;
        private int _currentPositionInHistory;

        internal static DataStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataStore();
                    ConfigureData();
                }
                return _instance;
            }
        }

        internal Uri BaseDirectoryFullName { get; private set; }

        internal string BaseRoute { get; private set; }


        private List<ExplorerObjectViewModel> AppDirectories { get; set; } = new();

        private List<ExplorerObjectViewModel> NavigationHistoryRoutesQueue { get; set; } = new();

        private ExplorerObjectViewModel CurrentExplorerObject { get; set; }


        private DataStore()
        {
            _currentPositionInHistory = 0;
            BaseDirectoryFullName = new Uri(Path.GetDirectoryName(Environment.CurrentDirectory));
            BaseRoute = new Uri(Environment.CurrentDirectory).AbsolutePath.Replace(BaseDirectoryFullName.AbsolutePath, "");
        }

        internal List<ExplorerObjectViewModel> GetRouteObjects(string route)
        {
            _currentPositionInHistory++;
            CurrentExplorerObject = AppDirectories.GetExplorerObjectByRoute(route);
            NavigationHistoryRoutesQueue.Add(CurrentExplorerObject);

            return CurrentExplorerObject.ObjectsInCurrentDirectory;
        }

        internal bool GetExplorerObjectFromHistory(bool routeDirection, out List<ExplorerObjectViewModel> explorerObjects)
        {
            explorerObjects = null;

            var countRouteQueue = NavigationHistoryRoutesQueue.Count - 1;

            if ((routeDirection && _currentPositionInHistory >= countRouteQueue) ||
                (!routeDirection && countRouteQueue <= 1)) return false;

            if (routeDirection) _currentPositionInHistory++;
            else _currentPositionInHistory--;

            explorerObjects = NavigationHistoryRoutesQueue[_currentPositionInHistory].ObjectsInCurrentDirectory;

            return explorerObjects != null;
        }

        private static void ConfigureData()
        {
            var baseObject = new ExplorerObjectViewModel(new DirectoryInfo(Environment.CurrentDirectory));

            _instance.AppDirectories.Add(baseObject);
            _instance.NavigationHistoryRoutesQueue.Add(baseObject);
            _instance.CreateTreeView(Environment.CurrentDirectory);
        }

        private void CreateTreeView(string path)
        {
            foreach (var folder in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
                _instance.AppDirectories.Add(new ExplorerObjectViewModel(new DirectoryInfo(folder)));          
        }

    }
}
